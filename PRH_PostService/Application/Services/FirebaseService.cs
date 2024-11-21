using Application.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

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
            var storageObject = await _storageClient.UploadObjectAsync(_bucketName, fileName, null, fileStream);
            return $"https://storage.googleapis.com/{_bucketName}/{storageObject.Name}";
        }
    }
}
