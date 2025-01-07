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
        IExpertProfileRepository expertProfileRepository,
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

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Errors.Add("Chứng chỉ không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật trạng thái chứng chỉ
                certificate.Status = 3; // Rejected
                certificate.VerifiedByAdminId = userId;
                certificate.VerifiedAt = DateTime.UtcNow.AddHours(7);
                certificate.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await certificateRepository.Update(certificate.CertificateId, certificate);

                // Cập nhật trạng thái hồ sơ chuyên gia
                var expertProfile = await expertProfileRepository.GetByIdAsync(certificate.ExpertProfileId);
                if (expertProfile != null)
                {
                    // Kiểm tra nếu có bất kỳ chứng chỉ nào được duyệt
                    var certificates = await certificateRepository.GetByExpertProfileIdAsync(expertProfile.ExpertProfileId);
                    if (!certificates.Any(c => c.Status == 1)) // No Verified certificates
                    {
                        expertProfile.Status = 2; // Rejected
                    }
                    expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);
                }

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chứng chỉ đã bị từ chối.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
