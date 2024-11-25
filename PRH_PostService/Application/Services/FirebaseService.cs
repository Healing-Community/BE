using Application.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Application.Services
{
    public class FirebaseService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseService(IConfiguration configuration)
        {
            _bucketName = configuration["FirebaseSettings:StorageBucket"]
                              ?? throw new InvalidOperationException("Firebase StorageBucket is not configured.");

            var credentialsPath = configuration["FirebaseSettings:CredentialsPath"]
                                  ?? throw new InvalidOperationException("Firebase CredentialsPath is not configured.");

            var googleCredential = GoogleCredential.FromFile(credentialsPath);
            _storageClient = StorageClient.Create(googleCredential);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            // Đảm bảo folderName chỉ thêm một lần và không bị lặp
            string folderName = "upload/";
            string sanitizedFileName = Path.GetFileName(fileName); // Loại bỏ các đường dẫn không cần thiết
            string objectName = $"{folderName}{sanitizedFileName}";

            // Thiết lập metadata để tệp có thể hiển thị đúng định dạng trên Firebase
            var storageObject = await _storageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = GetContentType(sanitizedFileName),
            }, fileStream);

            // Tạo token truy cập công khai (để hiển thị hình ảnh)
            string token = Guid.NewGuid().ToString();
            if (storageObject.Metadata == null)
            {
                storageObject.Metadata = new Dictionary<string, string>();
            }
            storageObject.Metadata["firebaseStorageDownloadTokens"] = token;
            await _storageClient.UpdateObjectAsync(storageObject);

            // Trả về URL công khai của hình ảnh
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={token}";
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}
