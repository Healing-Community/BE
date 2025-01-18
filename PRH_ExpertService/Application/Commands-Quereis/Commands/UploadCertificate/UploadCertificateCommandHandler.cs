using Application.Commands.UploadCertificate;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

public class UploadCertificateCommandHandler(
    IFirebaseStorageService firebaseStorageService,
    ICertificateRepository certificateRepository,
    IHttpContextAccessor httpContextAccessor,
    ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UploadCertificateCommand, BaseResponse<UploadCertificateResponse>>
{
    private static readonly List<string> ValidFileExtensions = new() { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 30 * 1024 * 1024; // 30MB
    private static readonly List<string> AllowedMimeTypes = new() { "application/pdf", "image/jpeg", "image/png" };

    public async Task<BaseResponse<UploadCertificateResponse>> Handle(UploadCertificateCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UploadCertificateResponse>
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
            response.Message = string.Join(" ", response.Errors);
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var userId = Authentication.GetUserIdFromHttpContext(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            response.Success = false;
            response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
            response.Message = string.Join(" ", response.Errors);
            response.StatusCode = StatusCodes.Status401Unauthorized;
            return response;
        }

        var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificationTypeId);
        if (certificateType == null)
        {
            response.Errors.Add($"Loại chứng chỉ với ID '{request.CertificationTypeId}' không hợp lệ.");
        }

        var file = request.File;
        if (file == null || file.Length == 0)
        {
            response.Errors.Add("File không hợp lệ. Vui lòng chọn một file hợp lệ để tải lên.");
        }
        else
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ValidFileExtensions.Contains(fileExtension))
            {
                response.Errors.Add($"Định dạng file '{fileExtension}' không hợp lệ. Chỉ chấp nhận các định dạng: {string.Join(", ", ValidFileExtensions)}.");
            }

            var mimeType = file.ContentType.ToLowerInvariant();
            if (!AllowedMimeTypes.Contains(mimeType))
            {
                response.Errors.Add("Định dạng MIME của tệp không được chấp nhận.");
            }

            if (file.Length > MaxFileSize)
            {
                response.Errors.Add($"Kích thước file hiện tại là {file.Length / 1024 / 1024}MB, vượt quá giới hạn cho phép là {MaxFileSize / 1024 / 1024}MB.");
            }
        }

        if (response.Errors.Any())
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            response.Success = false;
            response.Message = string.Join(" ", response.Errors);
            return response;
        }

        string? fileUrl = null;
        try
        {
            var fileName = $"{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";
            fileUrl = await firebaseStorageService.UploadFileAsync(file.OpenReadStream(), fileName, file.ContentType);

            var certificateId = Ulid.NewUlid().ToString();
            var certificate = new Certificate
            {
                CertificateId = certificateId,
                ExpertProfileId = userId,
                CertificateTypeId = request.CertificationTypeId,
                FileUrl = fileUrl,
                IssueDate = null,
                ExpirationDate = null,
                Status = 0,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            await certificateRepository.Create(certificate);

            response.Success = true;
            response.Message = "Tệp đã được tải lên thành công. Vui lòng hoàn tất thông tin cá nhân và chờ phê duyệt.";
            response.Data = new UploadCertificateResponse
            {
                FileUrl = fileUrl,
                CertificateId = certificateId
            };
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
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

            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Errors.Add($"Exception: {ex.Message}");
            response.Message = string.Join(" ", response.Errors);
        }

        return response;
    }
}
