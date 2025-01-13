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
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId})
            );  

            var userInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = appointment.UserId})
            ); 

            var ExpertInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                "UserService",
                async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = appointment.ExpertProfileId})
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
            },QueueName.AppointmentModerateQueue, cancellationToken);
            // Sync with report services
            await messagePublisher.PublishAsync(new SyncModerateAppointmentMessage
            {
                AppointmentId = appointment.AppointmentId,
                IsApprove = request.IsApprove
            }, QueueName.SyncAppointmentReportQueue, cancellationToken);

            return BaseResponse<string>.SuccessReturn("Duyệt báo cáo lịch hẹn thành công với trạng thái " + (request.IsApprove ? "Thành công" : "Thất bại"));

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
