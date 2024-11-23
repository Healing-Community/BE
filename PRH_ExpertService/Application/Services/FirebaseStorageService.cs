using Google.Cloud.Storage.V1;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;

namespace Application.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseStorageService(IConfiguration configuration)
        {
            _bucketName = configuration["FirebaseSettings:StorageBucket"]
                ?? throw new InvalidOperationException("Firebase StorageBucket is not configured.");

            var credentialsPath = configuration["FirebaseSettings:CredentialsPath"]
                ?? throw new InvalidOperationException("Firebase CredentialsPath is not configured.");

            var googleCredential = GoogleCredential.FromFile(credentialsPath);
            _storageClient = StorageClient.Create(googleCredential);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            string folderName = "upload_certificate/";
            string sanitizedFileName = Path.GetFileName(fileName);
            string objectName = $"{folderName}{sanitizedFileName}";

            var storageObject = await _storageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = contentType,
            }, fileStream);

            string token = Guid.NewGuid().ToString();
            storageObject.Metadata ??= new Dictionary<string, string>();
            storageObject.Metadata["firebaseStorageDownloadTokens"] = token;

            await _storageClient.UpdateObjectAsync(storageObject);

            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={token}";
        }
    }
}
