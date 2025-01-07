using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointments.GetAllAppointment;

public record GetAllAppointmentQuery : IRequest<BaseResponse<IEnumerable<Appointment>>>;