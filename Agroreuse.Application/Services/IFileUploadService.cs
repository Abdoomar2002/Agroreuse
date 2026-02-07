using Microsoft.AspNetCore.Http;

namespace Agroreuse.Application.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        void DeleteImage(string imagePath);
        bool IsValidImage(IFormFile file);
    }
}
