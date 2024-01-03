using AspNetDemo.Shared;

namespace AspNetDemo.Api.Services
{
    public interface IManageImage
    {
        Task<RequestResponse> Uploadfile(IFormFile file);
        Task<(byte[], string, string)> DownloadFile(string FileName);
    }
}
