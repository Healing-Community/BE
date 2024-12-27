using System;
using Application.Commands.UpdateAppointmentStatus;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.UpdateAppointment.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<UpdateAppointmentStatusCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                return BaseResponse<string>.NotFound("không tìm thấy thông tin lịch hẹn");
            }
            appointment.Status = request.Status;
            await appointmentRepository.Update(appointment.AppointmentId,appointment);
            return BaseResponse<string>.SuccessReturn("Cập nhật trạng thái lịch hẹn thành công");
        }
        catch (Exception e) 
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
