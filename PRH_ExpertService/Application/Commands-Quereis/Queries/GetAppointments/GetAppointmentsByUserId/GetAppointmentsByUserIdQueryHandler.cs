using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointments.GetAppointmentsByUserId;

public class GetAppointmentsByUserIdQueryHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAppointmentsByUserIdQuery, BaseResponse<IEnumerable<Appointment>>>
{
    public async Task<BaseResponse<IEnumerable<Appointment>>> Handle(GetAppointmentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await appointmentRepository.GetByUserIdAsync(request.UserId);
            return BaseResponse<IEnumerable<Appointment>>.SuccessReturn(appointments);
        }
        catch (Exception ex) 
        {
            return BaseResponse<IEnumerable<Appointment>>.InternalServerError(ex.Message);
            throw;
        }
    }
}
