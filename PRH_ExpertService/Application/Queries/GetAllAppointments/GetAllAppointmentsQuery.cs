using Application.Commons;
using Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.GetAllAppointments
{
    public record GetAllAppointmentsQuery : IRequest<BaseResponse<IEnumerable<Appointment>>>;
}
