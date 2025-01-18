using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.ApproveCertificate
{
    public class ApproveCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IExpertProfileRepository expertProfileRepository,
        ICertificateTypeRepository certificateTypeRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<ApproveCertificateCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(ApproveCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            // Xử lý lỗi hệ thống liên quan đến context
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

            // Xử lý lỗi liên quan đến dữ liệu đầu vào từ người dùng
            var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
            if (certificate == null)
            {
                response.Errors.Add($"Chứng chỉ với ID '{request.CertificateId}' không tồn tại.");
                response.Success = false;
                response.Message = string.Join(" ", response.Errors);
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return response;
            }

            try
            {
                certificate.Status = 1; // Verified
                certificate.VerifiedByAdminId = userId;
                certificate.VerifiedAt = DateTime.UtcNow.AddHours(7);
                certificate.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await certificateRepository.Update(certificate.CertificateId, certificate);

                // Cập nhật trạng thái hồ sơ chuyên gia
                var expertProfile = await expertProfileRepository.GetByIdAsync(certificate.ExpertProfileId);
                if (expertProfile != null)
                {
                    // Lấy danh sách các loại chứng chỉ bắt buộc
                    var mandatoryCertificateTypes = await certificateTypeRepository.GetMandatoryCertificateTypesAsync();

                    // Lấy danh sách chứng chỉ đã được duyệt của chuyên gia
                    var approvedCertificates = await certificateRepository.GetApprovedCertificatesByExpertIdAsync(expertProfile.ExpertProfileId);

                    // Kiểm tra nếu tất cả chứng chỉ bắt buộc đều đã được duyệt
                    bool allMandatoryCertificatesApproved = mandatoryCertificateTypes.All(mandatoryType =>
                        approvedCertificates.Any(c => c.CertificateTypeId == mandatoryType.CertificateTypeId));

                    if (allMandatoryCertificatesApproved)
                    {
                        expertProfile.Status = 1; // Approved
                        expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                        await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);
                    }
                }

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chứng chỉ đã được phê duyệt thành công.";
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống khi xử lý logic
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors);
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
