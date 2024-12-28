using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAppointments.GetAppointment;

public class GetAppointmentByPropertyQueryHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAppointmentByPropertyQuery, BaseResponse<Appointment>>
{
    public async Task<BaseResponse<Appointment>> Handle(GetAppointmentByPropertyQuery request, CancellationToken cancellationToken)
    {
        try 
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                return BaseResponse<Appointment>.NotFound("Không tìm thấy lịch hẹn");
            }
            return BaseResponse<Appointment>.SuccessReturn(appointment);
        }
        catch (Exception e)
        {
            return BaseResponse<Appointment>.InternalServerError(e.Message);
        }
    }
}
