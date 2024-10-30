using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.UpdateCertificate
{
    public class UpdateCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        ICertificateTypeRepository certificateTypeRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateCertificateCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
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

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Chứng chỉ không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                if (!string.IsNullOrEmpty(request.CertificateTypeId))
                {
                    var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                    if (certificateType == null)
                    {
                        response.Success = false;
                        response.Message = "Loại chứng chỉ không hợp lệ.";
                        response.StatusCode = 400;
                        return response;
                    }
                    certificate.CertificateTypeId = request.CertificateTypeId;
                }

                certificate.IssueDate = request.IssueDate ?? certificate.IssueDate;
                certificate.ExpirationDate = request.ExpirationDate ?? certificate.ExpirationDate;
                certificate.UpdatedAt = DateTime.UtcNow;

                await certificateRepository.Update(certificate.CertificateId, certificate);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Cập nhật chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
