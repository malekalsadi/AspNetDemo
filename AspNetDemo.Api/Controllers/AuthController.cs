using AspNetDemo.Api.Models;
using AspNetDemo.Api.Services;
using AspNetDemo.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Policy;

namespace AspNetDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel data)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterAsync(data);
                if (result.code == 200)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some Properties are not Valid");
        }

        [HttpGet("SetUp")]
        public async Task<IActionResult> StartUp()
        {
            var result = await _userService.InitializeAdmin();
            return Ok(result);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel data)
        {

            if(ModelState.IsValid)
            {
                var result = await _userService.SignInAsync(data);
                if(result.code == 200)
                {
                    return Ok(result);
                }
                return BadRequest(result);

            }
            return BadRequest("Some Properties are not Valid");
        }

        [HttpPost("Document")]
        public async Task<IActionResult> SubmitDocument([FromBody] IFormFile file)
        {
            var result =await _userService.UploadDocument(file);
            return Ok();    
        }
    }
    
}
