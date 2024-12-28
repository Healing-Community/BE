using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetAppointmentsByUser
{
    public record GetAppointmentsByUserQuery(string UserId) : IRequest<BaseResponse<IEnumerable<AppointmentResponseForUserDto>>>;
}
