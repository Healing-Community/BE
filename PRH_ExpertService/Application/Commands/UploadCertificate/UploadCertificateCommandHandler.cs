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
    ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UploadCertificateCommand, DetailBaseResponse<UploadCertificateResponse>>
{
    private static readonly List<string> ValidFileExtensions = new() { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 30 * 1024 * 1024; // 30MB
    private static readonly List<string> AllowedMimeTypes = new() { "application/pdf", "image/jpeg", "image/png" };

    public async Task<DetailBaseResponse<UploadCertificateResponse>> Handle(UploadCertificateCommand request, CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<UploadCertificateResponse>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7),
            Errors = new List<ErrorDetail>()
        };

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            response.Success = false;
            response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var userId = Authentication.GetUserIdFromHttpContext(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            response.Success = false;
            response.Message = "Không thể xác định UserId từ yêu cầu.";
            response.StatusCode = StatusCodes.Status401Unauthorized;
            return response;
        }

        var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificationTypeId);
        if (certificateType == null)
        {
            response.Errors.Add(new ErrorDetail
            {
                Message = $"Loại chứng chỉ với ID '{request.CertificationTypeId}' không hợp lệ.",
                Field = "CertificationTypeId"
            });
        }

        var file = request.File;
        if (file == null || file.Length == 0)
        {
            response.Errors.Add(new ErrorDetail
            {
                Message = "File không hợp lệ. Vui lòng chọn một file hợp lệ để tải lên.",
                Field = "File"
            });
        }
        else
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!ValidFileExtensions.Contains(fileExtension))
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Định dạng file '{fileExtension}' không hợp lệ. Chỉ chấp nhận các định dạng: {string.Join(", ", ValidFileExtensions)}.",
                    Field = "FileExtension"
                });
            }

            var mimeType = file.ContentType.ToLowerInvariant();
            if (!AllowedMimeTypes.Contains(mimeType))
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Định dạng MIME của tệp không được chấp nhận.",
                    Field = "MimeType"
                });
            }

            if (file.Length > MaxFileSize)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Kích thước file hiện tại là {file.Length / 1024 / 1024}MB, vượt quá giới hạn cho phép là {MaxFileSize / 1024 / 1024}MB.",
                    Field = "FileSize"
                });
            }
        }

        if (response.Errors.Any())
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            response.Success = false;
            response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
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
            response.Message = "Tệp đã được tải lên thành công.";
            response.Data = new UploadCertificateResponse
            {
                FileUrl = fileUrl,
                CertificateId = certificateId
            };
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
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = $"Lỗi khi xóa file trên Firebase: {deleteEx.Message}",
                        Field = "Cleanup"
                    });
                }
            }

            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.";
            response.Errors.Add(new ErrorDetail
            {
                Message = ex.Message,
                Field = "Exception"
            });
        }

        return response;
    }
}
