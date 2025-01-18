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

namespace Application.Commands_Quereis.Commands.ReportAppoinmet;

public class ReportAppointmentCommandHandler(IGrpcHelper grpcHelper, IMessagePublisher messagePublisher, IHttpContextAccessor accessor, IAppointmentRepository appointmentRepository) : IRequestHandler<ReportAppointmentCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ReportAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var appointment = await appointmentRepository.GetByPropertyAsync(x => x.AppointmentId == request.AppoinmtentId && x.UserId == userId && x.Status == (int)AppointmentStatusEnum.Completed);
            if (appointment == null)
            {
                return BaseResponse<string>.SuccessReturn("Lịch hẹn không tồn tại hoặc chưa hoàn thành");
            }

            appointment.Status = (int)AppointmentStatusEnum.Reported;

            await appointmentRepository.Update(appointment.AppointmentId, appointment);

            //Grpc to user service to get user information

            var userReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );

            var expertReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = appointment.ExpertProfileId })
            );

            // Send to report service through message queue
            await messagePublisher.PublishAsync(new ReportAppointmentMessage
            {
                UserId = userId,
                UserName = userReply?.UserName,
                UserEmail = userReply?.Email,
                AppoinmtentDate = appointment.AppointmentDate,
                EndTime = appointment.EndTime,
                StartTime = appointment.StartTime,
                ExpertEmail = expertReply?.Email,
                ExpertName = expertReply?.UserName,
                AppointmentId = appointment.AppointmentId,
                ReportDescription = request.ReportDescription
            }, QueueName.AppointmentReportQueue, cancellationToken);

            var userMail = new SendMailMessage
            {
                To = userReply.Email,
                Subject = "Báo cáo lịch hẹn",
                Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo Lịch Hẹn</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Báo cáo về lịch hẹn vào ngày <strong>{appointment.AppointmentDate}</strong>, thời gian từ <strong>{appointment.StartTime}</strong> đến <strong>{appointment.EndTime}</strong> của bạn đã được ghi nhận và sẽ được xử lý trong thời gian sớm nhất! 
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Cảm ơn bạn đã sử dụng dịch vụ của <strong>Healing Community</strong>.
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
    "
            };

            var expertMail = new SendMailMessage
            {
                To = expertReply.Email,
                Subject = "Báo cáo lịch hẹn",
                Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo Lịch Hẹn</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Lịch hẹn của bạn từ <strong>'{appointment.StartTime}' đến '{appointment.EndTime}'</strong> vào ngày <strong>'{appointment.AppointmentDate}'</strong> đã bị báo cáo bởi <strong>{userReply.UserName}</strong>.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Vui lòng cung cấp video hoặc hình ảnh làm bằng chứng cho sự việc này. Để xử lý báo cáo, bạn cần:
        </p>
        <ul style=""font-size: 17px; line-height: 1.8; color: #444;"">
            <li>Đính kèm đường link hoặc file hình ảnh, video.</li>
            <li>Trả lời email này kèm thông tin bổ sung.</li>
        </ul>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Nội dung báo cáo: <em>{request.ReportDescription}</em>
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
"
            };

            await messagePublisher.PublishAsync(userMail, QueueName.MailQueue, cancellationToken);
            await messagePublisher.PublishAsync(expertMail, QueueName.MailQueue, cancellationToken);

            return BaseResponse<string>.SuccessReturn("Báo cáo của bạn đã được ghi nhận và sẽ được xử lý trong thời gian sớm nhất");

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
