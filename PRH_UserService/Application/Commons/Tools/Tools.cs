using System;
using System.IO;
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
            string outputFormat = "png")
        {
            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentException("The uploaded file is null or empty.");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy the file to a memory stream
                    formFile.CopyTo(memoryStream);

                    // Load the image using ImageSharp
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (var image = Image.Load(memoryStream))
                    {
                        // Determine the size for cropping (square based on the shortest dimension)
                        int squareSize = Math.Min(image.Width, image.Height);

                        // Crop the image to a square, centering on the middle of the image
                        image.Mutate(x => x.Crop(new Rectangle(
                            (image.Width - squareSize) / 2, 
                            (image.Height - squareSize) / 2, 
                            squareSize, 
                            squareSize
                        )));

                        // Resize the image to the maximum dimensions
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(maxWidth, maxHeight),
                            Mode = ResizeMode.Max
                        }));

                        using (var compressedStream = new MemoryStream())
                        {
                            // Configure the encoder based on the output format
                            IImageEncoder encoder;
                            if (outputFormat.ToLower() == "png")
                            {
                                encoder = new PngEncoder
                                {
                                    CompressionLevel = (PngCompressionLevel)compressionLevel
                                };
                            }
                            else if (outputFormat.ToLower() == "jpeg")
                            {
                                encoder = new JpegEncoder
                                {
                                    Quality = 100 - compressionLevel * 10 // Map compression level to JPEG quality
                                };
                            }
                            else
                            {
                                throw new ArgumentException("Invalid output format. Supported formats: png, jpeg.");
                            }

                            // Save the processed image to the compressed stream
                            image.Save(compressedStream, encoder);

                            // Convert the compressed image to a Base64 string
                            string base64ImageRepresentation = Convert.ToBase64String(compressedStream.ToArray());

                            // Add the MIME type for the output format
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
