using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Domain.Entities;

namespace Application.Commands.UploadFile
{
    public class UploadFileCommandHandler(
        IFirebaseStorageService firebaseStorageService,
        ICertificateRepository certificateRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor,
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UploadFileCommand, BaseResponse<string>>
    {
        private static readonly List<string> ValidFileExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly List<string> AllowedMimeTypes = ["application/pdf", "image/jpeg", "image/png"];

        public async Task<BaseResponse<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy hồ sơ của chuyên gia với ID: {request.ExpertId}. Vui lòng kiểm tra lại ID.";
                    response.StatusCode = 404;
                    return response;
                }

                var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificationTypeId);
                if (certificateType == null)
                {
                    response.Success = false;
                    response.Message = $"Loại chứng chỉ với ID '{request.CertificationTypeId}' không hợp lệ. Vui lòng kiểm tra và thử lại.";
                    response.StatusCode = 400;
                    return response;
                }

                var file = request.File;
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File không hợp lệ. Vui lòng chọn một file hợp lệ để tải lên.";
                    response.StatusCode = 400;
                    return response;
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!ValidFileExtensions.Contains(fileExtension))
                {
                    response.Success = false;
                    response.Message = $"Định dạng file '{fileExtension}' không hợp lệ. Chỉ chấp nhận các định dạng: {string.Join(", ", ValidFileExtensions)}.";
                    response.StatusCode = 400;
                    return response;
                }

                var mimeType = file.ContentType.ToLowerInvariant();
                if (!AllowedMimeTypes.Contains(mimeType))
                {
                    response.Success = false;
                    response.Message = "Định dạng MIME của tệp không được chấp nhận.";
                    response.StatusCode = 400;
                    return response;
                }

                if (file.Length > MaxFileSize)
                {
                    response.Success = false;
                    response.Message = $"Kích thước file hiện tại là {file.Length / 1024 / 1024}MB, vượt quá giới hạn cho phép là 5MB. Vui lòng chọn file nhỏ hơn.";
                    response.StatusCode = 400;
                    return response;
                }

                var fileName = $"{Ulid.NewUlid()}_{Path.GetFileName(file.FileName)}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                string fileUrl = await firebaseStorageService.UploadFileAsync(memoryStream, fileName);

                var certificate = new Certificate
                {
                    CertificateId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = request.ExpertId,
                    CertificateTypeId = request.CertificationTypeId,
                    FileUrl = fileUrl,
                    IssueDate = DateTime.UtcNow,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await certificateRepository.Create(certificate);

                response.Success = true;
                response.Message = "Tệp đã được tải lên thành công.";
                response.Data = fileUrl;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu. Vui lòng thử lại sau.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
