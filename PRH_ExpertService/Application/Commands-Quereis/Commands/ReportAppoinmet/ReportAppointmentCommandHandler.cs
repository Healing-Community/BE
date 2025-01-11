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
                Body = $"Lịch hẹn với chuyên giai '{expertReply.UserName}' của bạn đã được báo cáo thành công và sẽ được xử lý trong thời gian sớm nhất"
            };
            var expertMail = new SendMailMessage
            {
                To = expertReply.Email,
                Subject = "Báo cáo lịch hẹn",
                Body = $"Lịch hẹn từ '{appointment.StartTime}' đến '{appointment.EndTime}' vào ngày '{appointment.AppointmentDate}' của bạn đã bị báo cáo bởi {userReply.UserName}. Vui lòng cung cấp thông tin video hoặc hình ảnh để chứng minh sự việc. \n Hãy trả lời tin nhắn này và gửi đính kèm đường link hoặc file hình ảnh, video để được duyệt. \n Nội dung báo cáo: {request.ReportDescription} \n Xin cảm ơn"
            };

            return BaseResponse<string>.SuccessReturn("Báo cáo của bạn đã được ghi nhận và sẽ được xử lý trong thời gian sớm nhất");  

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
