using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAppointmentsByExpert
{
    public record GetAppointmentsByExpertQuery(string ExpertProfileId) : IRequest<BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>>;
}
