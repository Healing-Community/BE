using Application.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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
            // Nén hình ảnh trước khi upload
            using var compressedStream = new MemoryStream();
            CompressImage(fileStream, compressedStream);
            // Đặt lại con trỏ stream về đầu trước khi upload
            compressedStream.Position = 0;

            string filePath = $"upload/{fileName}";

            // Tự động xác định MIME type
            string contentType = GetContentType(fileName);
            var storageObject = await _storageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = filePath,
                ContentType = contentType
            }, compressedStream);

            return $"https://storage.googleapis.com/{_bucketName}/{storageObject.Name}";
        }

        private void CompressImage(Stream inputStream, Stream outputStream)
        {
            using var image = Image.Load(inputStream);

            // Tùy chỉnh kích thước và chất lượng ảnh
            int maxWidth = 1920; 
            int maxHeight = 1080; 
            var resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            };

            image.Mutate(x => x.Resize(resizeOptions));

            // Lưu ảnh đã nén với mức chất lượng
            var encoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level6, // Mức độ nén (Level1 đến Level9)
            };
            image.Save(outputStream, encoder);
        }

        private string GetContentType(string fileName)
        {
            var contentTypes = new Dictionary<string, string>
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".webp", "image/webp" }
            };

            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return contentTypes.TryGetValue(fileExtension, out var contentType) ? contentType : "application/octet-stream";
        }
    }
}
