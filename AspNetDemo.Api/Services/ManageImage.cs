using AspNetDemo.Shared;
using System.Text;


namespace AspNetDemo.Api.Services
{
    public class ManageImage : IManageImage
    {
        private readonly IConfiguration _config;
        public ManageImage(IConfiguration configuration) 
        {
            _config = configuration;
        }
        public async Task<(byte[], string, string)> DownloadFile(string FileName)
        {
            throw new NotImplementedException();
        }

        public async Task<RequestResponse> Uploadfile(IFormFile file)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(_config["StoreFiles:PdfPaths"],
                    Path.GetRandomFileName());

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                return new RequestResponse
                {
                    code = 200,
                    message = "Uploaded successfully"
                };
            }
            return new RequestResponse
            {
                code = 408,
                message = "File size is Zero"
            };

        }
    }
}
