using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.UploadProfileImage
{
    public class UploadProfileImageCommandHandler(
        IFirebaseStorageService firebaseStorageService,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UploadProfileImageCommand, BaseResponse<string>>
    {
        private static readonly List<string> ValidFileExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };
        private static readonly List<string> AllowedMimeTypes = new() { "image/jpeg", "image/png", "image/gif" };

        public async Task<BaseResponse<string>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                response.Success = false;
                response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            var userId = Authentication.GetUserIdFromHttpContext(httpContext);
            if (string.IsNullOrEmpty(userId))
            {
                response.Success = false;
                response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            }

            var file = request.File;
            if (file == null || file.Length == 0)
            {
                response.Errors.Add("File không hợp lệ.");
            }
            else
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!ValidFileExtensions.Contains(extension))
                {
                    response.Errors.Add("Định dạng file không hợp lệ.");
                }

                if (!AllowedMimeTypes.Contains(file.ContentType))
                {
                    response.Errors.Add("Loại file không được phép.");
                }

                // Giới hạn dung lượng tối đa (ví dụ: 30MB)
                if (file.Length > 30 * 1024 * 1024)
                {
                    response.Errors.Add("Dung lượng ảnh vượt quá giới hạn cho phép (30MB).");
                }
            }

            if (response.Errors.Any())
            {
                response.Success = false;
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return response;
            }

            string? fileUrl = null;
            try
            {
                var fileName = $"{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";
                fileUrl = await firebaseStorageService.UploadImageAsync(file.OpenReadStream(), fileName, file.ContentType);

                response.Success = true;
                response.Data = fileUrl;
                response.Message = "Upload ảnh thành công.";
                response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                // Xóa file trên Firebase nếu đã upload
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    try
                    {
                        await firebaseStorageService.DeleteFileAsync(fileUrl);
                    }
                    catch (Exception deleteEx)
                    {
                        response.Errors.Add($"Lỗi khi xóa file trên Firebase: {deleteEx.Message}");
                    }
                }

                response.Success = false;
                response.Errors.Add($"Exception: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
