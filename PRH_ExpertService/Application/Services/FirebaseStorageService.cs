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

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var storageObject = await _storageClient.UploadObjectAsync(
                    bucket: _bucketName,
                    objectName: fileName,
                    contentType: null,
                    source: fileStream,
                    options: new UploadObjectOptions
                    {
                        PredefinedAcl = PredefinedObjectAcl.PublicRead
                    });

                return $"https://storage.googleapis.com/{_bucketName}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file to Firebase: " + ex.Message);
            }
        }
    }
}
