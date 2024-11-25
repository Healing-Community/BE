using Google.Cloud.Storage.V1;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Application.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;
        private readonly List<string> _allowedImageExtensions = new() { ".jpg", ".jpeg", ".png" };

        public FirebaseStorageService(IConfiguration configuration)
        {
            _bucketName = configuration["FirebaseSettings:StorageBucket"]
                ?? throw new InvalidOperationException("Firebase StorageBucket is not configured.");

            var credentialsPath = configuration["FirebaseSettings:CredentialsPath"]
                ?? throw new InvalidOperationException("Firebase CredentialsPath is not configured.");

            var googleCredential = GoogleCredential.FromFile(credentialsPath);
            _storageClient = StorageClient.Create(googleCredential);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Kiểm tra định dạng file
            if (!_allowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException("Chỉ hỗ trợ upload các định dạng ảnh: .jpg, .jpeg, .png");
            }

            // Giới hạn dung lượng file (ví dụ: 30MB)
            if (fileStream.Length > 30 * 1024 * 1024)
            {
                throw new ArgumentException("Dung lượng ảnh vượt quá giới hạn cho phép (30MB).");
            }

            // Xử lý ảnh
            var processedImage = await ProcessImageAsync(fileStream, extension);

            // Upload file lên Firebase
            string folderName = "upload_expert_image/";
            string sanitizedFileName = Path.GetFileName(fileName);
            string objectName = $"{folderName}{sanitizedFileName}";

            var storageObject = await _storageClient.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = processedImage.ContentType,
            }, processedImage.Stream);

            string token = Guid.NewGuid().ToString();
            storageObject.Metadata ??= new Dictionary<string, string>();
            storageObject.Metadata["firebaseStorageDownloadTokens"] = token;

            await _storageClient.UpdateObjectAsync(storageObject);

            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={token}";
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // Giới hạn dung lượng file (ví dụ: 30MB)
            if (fileStream.Length > 30 * 1024 * 1024)
            {
                throw new ArgumentException("Dung lượng file vượt quá giới hạn cho phép (30MB).");
            }

            // Upload file lên Firebase
            string folderName = "upload_certificates/";
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

        private async Task<(Stream Stream, string ContentType)> ProcessImageAsync(Stream inputStream, string extension)
        {
            inputStream.Position = 0; // Đảm bảo stream bắt đầu từ đầu

            using var image = await Image.LoadAsync(inputStream);
            var outputStream = new MemoryStream();

            // Thiết lập kích thước tối đa
            var maxWidth = 1920;
            var maxHeight = 1080;

            // Resize ảnh nếu kích thước lớn hơn
            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));
            }

            // Nén ảnh dựa trên định dạng
            IImageEncoder encoder;
            string contentType;

            if (extension == ".jpg" || extension == ".jpeg")
            {
                encoder = new JpegEncoder
                {
                    Quality = 80 // Chất lượng ảnh sau khi nén (từ 1-100)
                };
                contentType = "image/jpeg";
            }
            else if (extension == ".png")
            {
                encoder = new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.Level6 // Mức độ nén (Level1 - Level9)
                };
                contentType = "image/png";
            }
            else
            {
                throw new ArgumentException("Định dạng ảnh không được hỗ trợ.");
            }

            await image.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;
            return (outputStream, contentType);
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            // Tách tên đối tượng từ URL
            var bucketPath = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/";
            if (!fileUrl.StartsWith(bucketPath))
            {
                throw new ArgumentException("URL không hợp lệ hoặc không thuộc bucket Firebase của bạn.");
            }

            var objectName = Uri.UnescapeDataString(fileUrl.Replace(bucketPath, "").Split("?")[0]);

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // File không tồn tại, không cần xóa
            }
        }

    }
}
