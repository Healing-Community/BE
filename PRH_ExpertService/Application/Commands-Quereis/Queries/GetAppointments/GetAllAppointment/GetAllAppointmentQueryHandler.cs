using System;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointments.GetAllAppointment;

public class GetAllAppointmentQueryHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAllAppointmentQuery, BaseResponse<IEnumerable<Appointment>>>
{
    public async Task<BaseResponse<IEnumerable<Appointment>>> Handle(GetAllAppointmentQuery request, CancellationToken cancellationToken)
    {
        var appointments = await appointmentRepository.GetsAsync();
        return BaseResponse<IEnumerable<Appointment>>.SuccessReturn(appointments);
    }
}
