using Application.Commands.UpdateAppointmentStatus;
using Application.Commands.UpdateAvailability.UpdateAvailabilityStatus;
using Application.Commands_Quereis.Queries.GetAppointmentById;
using Application.Queries.GetExpertAvailbilityByAppointmentId;
using Application.Queries.GetExpertProfile;
using Grpc.Core;
using MediatR;

public class ExpertService(ISender sender) : ExpertPaymentService.ExpertService.ExpertServiceBase
{
    public override Task<ExpertPaymentService.GetAppointmentsResponse> GetAppointments(ExpertPaymentService.GetAppointmentsRequest request, ServerCallContext context)
    {
        var appointment = sender.Send(new GetAppointmentByIdQuery(request.AppointmentId));
        appointment.Wait();
        var appointmentResult = appointment.Result.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));
        
        var expertProfile = sender.Send(new GetExpertProfileQuery(appointmentResult.ExpertProfileId));
        expertProfile.Wait();
        var expertProfileResult = expertProfile.Result.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin chuyên gia"));

        var expertAlbility = sender.Send(new GetExpertAvailbilityByAppointmentIdQuery(appointmentResult.AppointmentId));
        expertAlbility.Wait();
        var expertAvailabilityResult = expertAlbility.Result.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));
        return Task.FromResult(new ExpertPaymentService.GetAppointmentsResponse{
            Amount = expertAvailabilityResult.Amount,
            AppointmentDate = appointmentResult.AppointmentDate.ToString(),
            EndTime = appointmentResult.EndTime.ToString(),
            StartTime = appointmentResult.StartTime.ToString(),
            ExpertEmail = appointmentResult.ExpertEmail,
            ExpertName = expertProfileResult.Fullname,
        });
    }
    public override Task<ExpertPaymentService.UpdateResponse> UpdateAppointment(ExpertPaymentService.GetAppointmentsRequest request, ServerCallContext context)
    {
        //Update status for appointment and expert availability
        var appointmentId = request.AppointmentId;
        var status = request.Status;
        
        var updateAppointmentStatus = sender.Send(new UpdateAppointmentStatusCommand(appointmentId, status));
        updateAppointmentStatus.Wait();
        var updateAppointmentStatusResponse = updateAppointmentStatus.Result ?? throw new RpcException(new Status(StatusCode.Internal, "Cập nhật trạng thái lịch hẹn thất bại"));
        
        return Task.FromResult(new ExpertPaymentService.UpdateResponse{
            IsSucess = updateAppointmentStatusResponse.Success
        });
    }
    public override Task<ExpertPaymentService.UpdateResponse> UpdateExpertAvailability(ExpertPaymentService.GetAppointmentsRequest request, ServerCallContext context)
    {
        //Update status for expert availability
        var appointmentId = request.AppointmentId;
        var status = request.Status;
        
        var updateExpertAvailabilityStatus = sender.Send(new UpdateAvailabilityStatusCommand(appointmentId, status));
        updateExpertAvailabilityStatus.Wait();
        var updateExpertAvailabilityStatusResponse = updateExpertAvailabilityStatus.Result ?? throw new RpcException(new Status(StatusCode.Internal, "Cập nhật trạng thái lịch hẹn thất bại"));
        
        return Task.FromResult(new ExpertPaymentService.UpdateResponse{
            IsSucess = updateExpertAvailabilityStatusResponse.Success
        });
    }
}