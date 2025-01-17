using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants.AMQPMessage;
using Domain.Constants.AMQPMessage.MailMessage;
using Domain.Constants.AMQPMessage.Report;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commands_Quereis.Commands.ReportAppoinmet;

public class ReportAppointmentCommandHandler(IGrpcHelper grpcHelper,IMessagePublisher messagePublisher,IHttpContextAccessor accessor, IAppointmentRepository appointmentRepository) : IRequestHandler<ReportAppointmentCommand, BaseResponse<string>>
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

            await appointmentRepository.Update(appointment.AppointmentId,appointment);

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
                            <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
                        </div>
                        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo</h2>
                        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">Báo cáo của bạn đã được ghi nhận và sẽ được xử lí trong thời gian sớm nhất!</p>
                        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";
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
                            <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
                        </div>
                        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo</h2>
                        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">Lịch hẹn từ '{appointment.StartTime}' đến '{appointment.EndTime}' vào ngày '{appointment.AppointmentDate}' của bạn đã bị báo cáo bởi {userReply.UserName}. Vui lòng cung cấp thông tin video hoặc hình ảnh để chứng minh sự việc. \n Hãy trả lời tin nhắn này và gửi đính kèm đường link hoặc file hình ảnh, video để được duyệt. \n Nội dung báo cáo: {request.ReportDescription}</p>
                        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";
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
