using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Application.Commons.Tools
{
    public static class Tools
    {
        public static string ConvertImageToBase64(
            IFormFile formFile,
            int maxWidth = 500,
            int maxHeight = 500,
            int compressionLevel = 9,
            int quality = 30,
            string outputFormat = "jpeg")
        {
            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentException("The uploaded file is null or empty.");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    formFile.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using (var image = Image.Load(memoryStream))
                    {
                        // Cắt phần vuông giữa ảnh
                        int squareSize = Math.Min(image.Width, image.Height);
                        int xOffset = (image.Width - squareSize) / 2;
                        int yOffset = (image.Height - squareSize) / 2;

                        // Cắt ảnh vuông từ giữa
                        var croppedImage = image.Clone(ctx => ctx.Crop(new Rectangle(xOffset, yOffset, squareSize, squareSize)));

                        // Giảm kích thước ảnh vuông xuống để giảm dung lượng
                        int newMaxWidth = Math.Min(maxWidth, 800);
                        int newMaxHeight = Math.Min(maxHeight, 800);

                        // Tính toán kích thước mới giữ nguyên tỷ lệ
                        double scale = Math.Min(
                            (double)newMaxWidth / croppedImage.Width,
                            (double)newMaxHeight / croppedImage.Height
                        );
                        int newWidth = (int)(croppedImage.Width * scale);
                        int newHeight = (int)(croppedImage.Height * scale);

                        // Resize ảnh với kích thước mới
                        croppedImage.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(newWidth, newHeight),
                            Mode = ResizeMode.Max,
                            Sampler = KnownResamplers.Lanczos3
                        }));

                        using (var compressedStream = new MemoryStream())
                        {
                            IImageEncoder encoder;
                            if (outputFormat.ToLower() == "png")
                            {
                                encoder = new PngEncoder
                                {
                                    CompressionLevel = PngCompressionLevel.BestCompression,
                                    FilterMethod = PngFilterMethod.Adaptive
                                };
                            }
                            else if (outputFormat.ToLower() == "jpeg")
                            {
                                encoder = new JpegEncoder
                                {
                                    Quality = quality
                                };
                            }
                            else
                            {
                                throw new ArgumentException("Invalid output format. Supported formats: png, jpeg.");
                            }

                            croppedImage.Save(compressedStream, encoder);
                            string base64ImageRepresentation = Convert.ToBase64String(compressedStream.ToArray());
                            string mimeType = outputFormat.ToLower() == "png" ? "image/png" : "image/jpeg";
                            return $"data:{mimeType};base64,{base64ImageRepresentation}";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred while converting the image to Base64: {e.Message}", e);
            }
        }
    }
}
