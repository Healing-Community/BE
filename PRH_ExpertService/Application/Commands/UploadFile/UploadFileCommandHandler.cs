using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UploadFile
{
    public class UploadFileCommandHandler(
        IFirebaseStorageService firebaseStorageService,
        ICertificateRepository certificateRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UploadFileCommand, BaseResponse<string>>
    {
        private static readonly List<string> ValidFileExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

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
                    response.Message = "Context không hợp lệ.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                var file = request.File;
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File không hợp lệ.";
                    response.StatusCode = 400;
                    return response;
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!ValidFileExtensions.Contains(fileExtension))
                {
                    response.Success = false;
                    response.Message = $"Định dạng file không hợp lệ. Chỉ chấp nhận: {string.Join(", ", ValidFileExtensions)}.";
                    response.StatusCode = 400;
                    return response;
                }

                if (file.Length > MaxFileSize)
                {
                    response.Success = false;
                    response.Message = "Kích thước file vượt quá giới hạn 5MB.";
                    response.StatusCode = 400;
                    return response;
                }

                var fileName = $"{Ulid.NewUlid()}_{Path.GetFileName(file.FileName)}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                string fileUrl = await firebaseStorageService.UploadFileAsync(memoryStream, fileName);

                var certificate = new Domain.Entities.Certificate
                {
                    CertificateId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = request.ExpertId,
                    CertificateTypeId = "1",
                    FileUrl = fileUrl,
                    IssueDate = DateTime.UtcNow,
                    Status = 1
                };

                await certificateRepository.Create(certificate);

                response.Success = true;
                response.Message = "Tải lên tệp thành công.";
                response.Data = fileUrl;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình xử lý.";
                response.StatusCode = 500;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
