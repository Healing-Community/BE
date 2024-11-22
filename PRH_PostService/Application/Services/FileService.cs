using Application.Interfaces.Service;
using Application.Interfaces.Services;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Linq;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFirebaseStorageService _firebaseService;
        private readonly List<string> _allowedImageExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };

        public FileService(IFirebaseStorageService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Validate định dạng file
            if (!_allowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException("Chỉ hỗ trợ upload các định dạng ảnh: .jpg, .jpeg, .png, .gif");
            }

            // Validate kích thước file
            if (file.Length > 30 * 1024 * 1024) // 30MB
            {
                throw new ArgumentException("Dung lượng ảnh vượt quá giới hạn cho phép (30MB).");
            }

            // Nén ảnh nếu kích thước quá lớn
            await using var originalStream = file.OpenReadStream();
            Stream compressedStream = originalStream;
            if (file.Length > 1 * 1024 * 1024) // Nén ảnh nếu > 1MB
            {
                compressedStream = CompressImage(originalStream, extension);
            }

            // Upload file lên Firebase
            var fileName = $"upload/{Guid.NewGuid()}{extension}";
            return await _firebaseService.UploadFileAsync(compressedStream, fileName);
        }

        private Stream CompressImage(Stream inputStream, string extension)
        {
            using var image = Image.Load(inputStream);
            var outputStream = new MemoryStream();

            // Tùy chỉnh nén ảnh
            var encoder = new JpegEncoder
            {
                Quality = 75 // Chất lượng ảnh sau khi nén (từ 1-100)
            };

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(1920, 1080) // Resize về tối đa 1920x1080 pixel
            }));

            image.Save(outputStream, encoder);
            outputStream.Position = 0;
            return outputStream;
        }
    }
}
