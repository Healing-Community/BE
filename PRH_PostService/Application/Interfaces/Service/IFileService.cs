using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Service
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
