using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAppointments
{
    public record GetAppointmentsQuery(string ExpertProfileId) : IRequest<BaseResponse<IEnumerable<Appointment>>>;
}
