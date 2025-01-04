using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.RejectCertificate
{
    public class RejectCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<RejectCertificateCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(RejectCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Errors.Add("Chứng chỉ không tồn tại.");
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                certificate.Status = 3; // Rejected
                certificate.VerifiedByAdminId = userId;
                certificate.VerifiedAt = DateTime.UtcNow.AddHours(7);
                certificate.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await certificateRepository.Update(certificate.CertificateId, certificate);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chứng chỉ đã bị từ chối.";
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi từ chối chứng chỉ.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
