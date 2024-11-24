namespace Application.Interfaces.Services
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    }
}
