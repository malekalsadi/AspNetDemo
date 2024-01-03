using AspNetDemo.Api.Models;
using AspNetDemo.Shared;

namespace AspNetDemo.Api.Services
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterModel data);
        Task<AuthResponse> SignInAsync(SignInModel data);
        Task<RequestResponse> InitializeAdmin();
        Task<RequestResponse> UploadDocument(IFormFile file);
    }
}
