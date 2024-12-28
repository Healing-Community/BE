using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAppointmentByIdQuery, BaseResponse<Appointment>>
{
    public async Task<BaseResponse<Appointment>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                return BaseResponse<Appointment>.NotFound();
            }
            return BaseResponse<Appointment>.SuccessReturn(appointment);
        }
        catch (Exception e)
        {
            return BaseResponse<Appointment>.InternalServerError(e.Message);
            throw;
        }
    }
}
