using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;
using Google.Protobuf;
using Application.Interfaces.AMQP;
using Domain.Constants.AMQPMessage;

namespace Application.Commands.RejectCertificate
{
    public class RejectCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IMessagePublisher messagePublisher,
        IExpertProfileRepository expertProfileRepository,
        ICertificateTypeRepository certificateTypeRepository,
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

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Errors.Add("Chứng chỉ không tồn tại.");
                    response.Message = string.Join(" ", response.Errors);
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
                    // Lấy danh sách các loại chứng chỉ bắt buộc
                    var mandatoryCertificateTypes = await certificateTypeRepository.GetMandatoryCertificateTypesAsync();

                    // Lấy danh sách chứng chỉ đã được duyệt của chuyên gia
                    var approvedCertificates = await certificateRepository.GetApprovedCertificatesByExpertIdAsync(expertProfile.ExpertProfileId);

                    // Kiểm tra nếu tất cả chứng chỉ bắt buộc đều đã được duyệt
                    bool allMandatoryCertificatesApproved = mandatoryCertificateTypes.All(mandatoryType =>
                        approvedCertificates.Any(c => c.CertificateTypeId == mandatoryType.CertificateTypeId));

                    if (!allMandatoryCertificatesApproved)
                    {
                        expertProfile.Status = 2; // Rejected
                        expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                        await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);
                    }
                }

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chứng chỉ đã bị từ chối.";

                // Gửi email thông báo cho chuyên gia
                var expert = await expertProfileRepository.GetByIdAsync(certificate.ExpertProfileId);
                var certificatetype = await certificateTypeRepository.GetByIdAsync(certificate.CertificateTypeId);
                if (expert != null)
                {
                    var email = expert.Email;
                    var subject = "Chứng chỉ của bạn đã bị từ chối";
                    var body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #f44336; text-align: center; margin-top: 20px;"">Chứng chỉ bị từ chối</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Chứng chỉ <strong>{certificatetype.Name}</strong> của bạn đã bị từ chối bởi quản trị viên. 
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Vui lòng kiểm tra lại thông tin và nộp lại chứng chỉ mới để tiếp tục quy trình.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Cảm ơn bạn đã hợp tác và sử dụng dịch vụ của <strong>Healing Community</strong>.
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
";

                    await messagePublisher.PublishAsync(new SendMailMessage
                    {
                        To = email,
                        Subject = subject,
                        Body = body
                    }, QueueName.MailQueue, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors);
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return response;
        }
    }
}
