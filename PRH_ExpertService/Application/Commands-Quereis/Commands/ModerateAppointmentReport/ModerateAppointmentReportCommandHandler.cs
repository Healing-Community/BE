using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants.AMQPMessage;
using Domain.Constants.AMQPMessage.Report;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commands_Quereis.Commands.ModerateAppointmentReport;

public class ModerateAppointmentReportCommandHandler(IAppointmentRepository appointmentRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher, IGrpcHelper grpcHelper) : IRequestHandler<ModerateAppointmentReportCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ModerateAppointmentReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var appointment = await appointmentRepository.GetByPropertyAsync(x => x.AppointmentId == request.AppointmentId && x.Status == (int)AppointmentStatusEnum.Reported);
            if (appointment == null)
            {
                return BaseResponse<string>.SuccessReturn("Lịch hẹn không tồn tại hoặc chưa báo cáo");
            }
            // Update appointment status
            appointment.Status = request.IsApprove ? (int)AppointmentStatusEnum.ReportSuccess : (int)AppointmentStatusEnum.ReportFailed;

            await appointmentRepository.Update(appointment.AppointmentId, appointment);

            // Grpc call to get user info
            var ModeratorInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
            );

            var userInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = appointment.UserId })
            );

            var ExpertInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = appointment.ExpertProfileId })
            );
            if (ModeratorInfoReply == null || userInfoReply == null || ExpertInfoReply == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy thông tin người dùng");
            }

            // Moderate activity
            await messagePublisher.PublishAsync(new ModerateAppointmentMessage
            {
                AppointmentId = appointment.AppointmentId,
                ModeratorEmail = ModeratorInfoReply.Email,
                ModeratorId = userId,
                ModeratorName = ModeratorInfoReply.UserName,
                AppoinmtentDate = appointment.AppointmentDate,
                EndTime = appointment.EndTime,
                ExpertEmail = ExpertInfoReply.Email,
                ExpertName = ExpertInfoReply.UserName,
                IsApprove = request.IsApprove,
                StartTime = appointment.StartTime,
                UserEmail = userInfoReply.Email,
                UserName = userInfoReply.UserName
            }, QueueName.AppointmentModerateQueue, cancellationToken);
            // Sync with report services
            await messagePublisher.PublishAsync(new SyncModerateAppointmentMessage
            {
                AppointmentId = appointment.AppointmentId,
                IsApprove = request.IsApprove
            }, QueueName.SyncAppointmentReportQueue, cancellationToken);

            // Send mail to user
            var userMail = new SendMailMessage
            {
                To = userInfoReply.Email,
                Subject = "Kết quả báo cáo lịch hẹn",
                Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Kết quả xử lý lịch hẹn</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Lịch hẹn của bạn vào ngày <strong>{appointment.AppointmentDate}</strong>, từ <strong>{appointment.StartTime}</strong> đến <strong>{appointment.EndTime}</strong> đã được 
            <strong>{ModeratorInfoReply.UserName}</strong> 
            <strong style=""color: {(request.IsApprove ? "#4caf50" : "#ff0000")};"">
                {(request.IsApprove ? "duyệt" : "từ chối")}
            </strong> 
            sau khi được xem xét kỹ lưỡng từ phía chúng tôi.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            {(request.IsApprove
                ? "Cảm ơn bạn đã sử dụng dịch vụ của Healing Community. Chúc bạn có một lịch hẹn thành công!"
                : "Nếu bạn có bất kỳ thắc mắc nào hoặc cần hỗ trợ thêm, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi.")}
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
"

            };
            await messagePublisher.PublishAsync(userMail, QueueName.MailQueue, cancellationToken);

            // Send mail to expert

            var expertMail = new SendMailMessage
            {
                To = ExpertInfoReply.Email,
                Subject = "Kết quả báo cáo lịch hẹn",
                Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Kết quả xử lý lịch hẹn</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Lịch hẹn của bạn vào ngày <strong>{appointment.AppointmentDate}</strong>, từ <strong>{appointment.StartTime}</strong> đến <strong>{appointment.EndTime}</strong> đã được 
            <strong>{ModeratorInfoReply.UserName}</strong> 
            <strong style=""color: {(request.IsApprove ? "#4caf50" : "#ff0000")};"">
                {(request.IsApprove ? "duyệt" : "từ chối")}
            </strong> 
            sau khi được xem xét kỹ lưỡng từ phía chúng tôi.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            {(request.IsApprove
                ? "Cảm ơn bạn đã sử dụng dịch vụ của Healing Community. Chúc bạn có một lịch hẹn thành công!"
                : "Nếu bạn có bất kỳ thắc mắc nào hoặc cần hỗ trợ thêm, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi.")}
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
"
            };
            await messagePublisher.PublishAsync(expertMail, QueueName.MailQueue, cancellationToken);

            return BaseResponse<string>.SuccessReturn("Duyệt báo cáo lịch hẹn thành công với trạng thái " + (request.IsApprove ? "Thành công" : "Thất bại"));

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
