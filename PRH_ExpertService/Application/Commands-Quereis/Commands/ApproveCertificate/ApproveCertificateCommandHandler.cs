using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using System.Net.Mail;
using Domain.Constants.AMQPMessage;

namespace Application.Commands.ApproveCertificate
{
    public class ApproveCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IMessagePublisher messagePublisher,
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

                // Send mail to expert
                var expert = await expertProfileRepository.GetByIdAsync(certificate.ExpertProfileId);
                var certificateType = await certificateTypeRepository.GetByIdAsync(certificate.CertificateTypeId);
                if (expert != null)
                {
                    var message = new SendMailMessage
                    {
                        To = expert.Email,
                        Subject = "Chứng chỉ đã được phê duyệt",
                        Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Chứng chỉ đã được phê duyệt</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Chứng chỉ <strong>{certificate.CertificateType.Name}</strong> của bạn đã được phê duyệt. 
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Vui lòng kiểm tra thông tin tại hệ thống để đảm bảo rằng tất cả các chi tiết là chính xác.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>Healing Community</strong>.
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
"

                    };

                    await messagePublisher.PublishAsync(message, QueueName.MailQueue, cancellationToken);
                }

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
