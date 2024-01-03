using AspNetDemo.Api.Models;
using AspNetDemo.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AspNetDemo.Api.Services
{
    public class AuthService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IManageImage _ImageManage;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config, IManageImage manageImage)
        {
            _userManager = userManager;
            _config = config;
            _ImageManage = manageImage;
        }
        public async Task<AuthResponse> RegisterAsync(RegisterModel data)
        {
            if(data == null)
                throw new NullReferenceException("Register Data are null");

            if (data.Password != data.ConfirmPassword)
                return new AuthResponse
                {
                    message = "Confirm Password Failed",
                    code = 409
                };

            var user = new IdentityUser
            {
                UserName = data.UserName,
                Email = data.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, data.Password);

            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    message = "Registerated successfully",
                    code = 200,
                };
            }

            return new AuthResponse
            {
                message = "Registeration Failed",
                code = 300,
                Error = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<AuthResponse> SignInAsync(SignInModel data)
        {
            if (data == null)
                throw new NullReferenceException("Log In Data are null");

            var user = await _userManager.FindByEmailAsync(data.Email);
            if (user == null)
                return new AuthResponse
                {
                    message = "There is no user with that email address",
                    code = 401
                };

            var result = await _userManager.CheckPasswordAsync(user, data.Password);
            if (!result)
                return new AuthResponse
                {
                    message = "Password is incorrect",
                    code = 401
                };

            SecurityToken token = await GenerateToken(user);   
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResponse
            {
                message = tokenString,
                code = 200,
                ExpireDate = token.ValidTo
            };
        }
        private async Task<SecurityToken> GenerateToken(IdentityUser data)
        {
            string email = data.Email;
            string role = "user";
            if (email.EndsWith(".admin.com"))
                role = "Admin";
            var MyClaims = new[]
            {
                new Claim("userId",data.Id),
                new Claim(ClaimTypes.Role,role)
            };

            byte[] bytekey = Encoding.UTF8.GetBytes(_config["AuthSettings:Key"]);
            var securityKey = new SymmetricSecurityKey(bytekey);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            SecurityToken token = new JwtSecurityToken(
                issuer: _config.GetSection("AuthSettings:Issuer").Value,
                audience: _config.GetSection("AuthSettings:Audience").Value,
                claims: MyClaims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );
            return token;
        }
        public async Task<RequestResponse> InitializeAdmin()
        {
            var user = new User { UserName = "admin", Email = "admin@mail.admin.com" };
            var result = await _userManager.CreateAsync(user, "P@ssw0rd");
            if(result.Succeeded) { 
                return new RequestResponse
                {
                    code = 200,
                    message = "OK",
                };
            }
            return new RequestResponse { code = 300, message = "SetUp failed", Error = result.Errors.Select(e => e.Description) };
        }

        public async Task<RequestResponse> UploadDocument(IFormFile file)
        {
            return await _ImageManage.Uploadfile(file);
        }
    }
}
