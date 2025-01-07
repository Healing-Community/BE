using Application.Commands.UpdateAppointment.UpdateAppointmentStatus;
using Application.Commands.UpdateAvailability.UpdateAvailabilityStatus;
using Application.Commands_Quereis.Queries.GetAppointmentById;
using Application.Commands_Quereis.Queries.GetAppointments.GetAllAppointment;
using Application.Commands_Quereis.Queries.GetAppointments.GetAppointmentsByUserId;
using Application.Queries.GetAppointments;
using Application.Queries.GetExpertAvailbilityByAppointmentId;
using Application.Queries.GetExpertProfile;
using Domain.Entities;
using ExpertPaymentService;
using Grpc.Core;
using MediatR;

public class ExpertService(ISender sender) : ExpertPaymentService.ExpertService.ExpertServiceBase
{
    public async override Task<GetAppointmentsListResponse> GetAllAppointments(GetAppointmentsRequestRepeated request, ServerCallContext context)
    {
        // Lấy tất cả lịch hẹn với status khác 0 (pending payment)
        var appointments = new List<Appointment>();
        foreach (var appointmentId in request.AppointmentIds)
        {
            var appointmentTask = await sender.Send(new GetAppointmentByIdQuery(appointmentId));

            var appointmentResult = appointmentTask.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));

            appointments.Add(appointmentResult);
        }
        // Map thông tin lịch hẹn và số tiền
        var mappedAppointments = new List<GetAppointmentsResponse>();

        foreach (var appointment in appointments)
        {
            // Lấy thông tin số tiền của lịch hẹn từ bảng ExpertAvailability
            var expertAvailabilityTask = await sender.Send(new GetExpertAvailbilityByAppointmentIdQuery(appointment.AppointmentId));
            
            var expertAvailabilityResult = expertAvailabilityTask.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));

            var expertProfileTask = await sender.Send(new GetExpertProfileQuery(appointment.ExpertProfileId));

            // Tạo response cho mỗi lịch hẹn
            var appointmentResponse = new GetAppointmentsResponse
            {
                AppointmentId = appointment.AppointmentId,
                StartTime = appointment.StartTime.ToString(),
                EndTime = appointment.EndTime.ToString(),
                AppointmentDate = appointment.AppointmentDate.ToString(),
                ExpertEmail = appointment.ExpertEmail,
                Status = appointment.Status,
                ExpertName = expertProfileTask?.Data?.Fullname,
                UserId = appointment.UserId,
                ExpertId = appointment.ExpertProfileId,
                Amount = expertAvailabilityResult.Amount // Map số tiền vào response
            };

            mappedAppointments.Add(appointmentResponse);
        }

        // Trả về danh sách response
        return new GetAppointmentsListResponse
        {
            Appointments = { mappedAppointments }
        };
    }
    public async override Task<GetAppointmentsListResponse> GetAppointmentsByUser(GetAppointmentsByUserRequest request, ServerCallContext context)
    {
        // Lấy các lịch hẹn của user với status khác 0 (pending payment)
        var appointments = await sender.Send(new GetAppointmentsByUserIdQuery(request.UserId));
        var appointmentsResult = appointments.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn. Expert-Service"));

        // Map thông tin lịch hẹn và số tiền
        var mappedAppointments = new List<GetAppointmentsResponse>();

        foreach (var appointment in appointmentsResult)
        {
            // Lấy thông tin số tiền của lịch hẹn từ bảng ExpertAvailability
            var expertAvailabilityTask = await sender.Send(new GetExpertAvailbilityByAppointmentIdQuery(appointment.AppointmentId));
            
            var expertAvailabilityResult = expertAvailabilityTask.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));

            var expertProfileTask = await sender.Send(new GetExpertProfileQuery(appointment.ExpertProfileId));

            // Tạo response cho mỗi lịch hẹn
            var appointmentResponse = new GetAppointmentsResponse
            {
                AppointmentId = appointment.AppointmentId,
                StartTime = appointment.StartTime.ToString(),
                EndTime = appointment.EndTime.ToString(),
                AppointmentDate = appointment.AppointmentDate.ToString(),
                ExpertEmail = appointment.ExpertEmail,
                Status = appointment.Status,
                ExpertName = expertProfileTask?.Data?.Fullname,
                UserId = appointment.UserId,
                ExpertId = appointment.ExpertProfileId,
                Amount = expertAvailabilityResult.Amount // Map số tiền vào response
            };

            mappedAppointments.Add(appointmentResponse);
        }
        return new GetAppointmentsListResponse
        {
            Appointments = { mappedAppointments }
        };
    }
    public async override Task<GetAppointmentsListResponse> GetAppointmentsByExpert(GetAppointmentsByExpertRequest request, ServerCallContext context)
    {
        // Lấy các lịch hẹn của chuyên gia với status khác 0 (pending payment)
        var appointments = await sender.Send(new GetAppointmentsQuery(request.ExpertId));
        var appointmentsResult = appointments.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn. Expert-Service"));

        // Map thông tin lịch hẹn và số tiền
        var mappedAppointments = new List<GetAppointmentsResponse>();

        foreach (var appointment in appointmentsResult)
        {
            // Lấy thông tin số tiền của lịch hẹn từ bảng ExpertAvailability
            var expertAvailabilityTask = await sender.Send(new GetExpertAvailbilityByAppointmentIdQuery(appointment.AppointmentId));
            
            var expertAvailabilityResult = expertAvailabilityTask.Data ?? throw new RpcException(new Status(StatusCode.NotFound, "không tìm thấy thông tin lịch hẹn"));

            var expertProfileTask = await sender.Send(new GetExpertProfileQuery(appointment.ExpertProfileId));

            // Tạo response cho mỗi lịch hẹn
            var appointmentResponse = new ExpertPaymentService.GetAppointmentsResponse
            {
                AppointmentId = appointment.AppointmentId,
                StartTime = appointment.StartTime.ToString(),
                EndTime = appointment.EndTime.ToString(),
                AppointmentDate = appointment.AppointmentDate.ToString(),
                ExpertEmail = appointment.ExpertEmail,
                Status = appointment.Status,
                ExpertName = expertProfileTask?.Data?.Fullname,
                UserId = appointment.UserId,
                ExpertId = appointment.ExpertProfileId,
                Amount = expertAvailabilityResult.Amount // Map số tiền vào response
            };

            mappedAppointments.Add(appointmentResponse);
        }

        // Trả về danh sách response
        return new GetAppointmentsListResponse
        {
            Appointments = { mappedAppointments }
        };
    }

    public override Task<GetAppointmentsResponse> GetAppointments(GetAppointmentsRequest request, ServerCallContext context)
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
        return Task.FromResult(new GetAppointmentsResponse
        {
            Amount = expertAvailabilityResult.Amount,
            AppointmentDate = appointmentResult.AppointmentDate.ToString(),
            EndTime = appointmentResult.EndTime.ToString(),
            StartTime = appointmentResult.StartTime.ToString(),
            ExpertEmail = appointmentResult.ExpertEmail,
            ExpertName = expertProfileResult.Fullname,
            ExpertId = appointmentResult.ExpertProfileId,
            UserId = appointmentResult.UserId,
        });
    }
    public override Task<UpdateResponse> UpdateAppointment(GetAppointmentsRequest request, ServerCallContext context)
    {
        //Update status for appointment and expert availability
        var appointmentId = request.AppointmentId;
        var status = request.Status;

        var updateAppointmentStatus = sender.Send(new UpdateAppointmentStatusCommand(appointmentId, status));
        updateAppointmentStatus.Wait();
        var updateAppointmentStatusResponse = updateAppointmentStatus.Result ?? throw new RpcException(new Status(StatusCode.Internal, "Cập nhật trạng thái lịch hẹn thất bại"));

        return Task.FromResult(new UpdateResponse
        {
            IsSucess = updateAppointmentStatusResponse.Success
        });
    }
    public override Task<UpdateResponse> UpdateExpertAvailability(GetAppointmentsRequest request, ServerCallContext context)
    {
        //Update status for expert availability
        var appointmentId = request.AppointmentId;
        var status = request.Status;

        var updateExpertAvailabilityStatus = sender.Send(new UpdateAvailabilityStatusCommand(appointmentId, status));
        updateExpertAvailabilityStatus.Wait();
        var updateExpertAvailabilityStatusResponse = updateExpertAvailabilityStatus.Result ?? throw new RpcException(new Status(StatusCode.Internal, "Cập nhật trạng thái lịch hẹn thất bại"));

        return Task.FromResult(new UpdateResponse
        {
            IsSucess = updateExpertAvailabilityStatusResponse.Success
        });
    }

}