using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointments.GetAppointmentsByUserId;

public record GetAppointmentsByUserIdQuery(string UserId) : IRequest<BaseResponse<IEnumerable<Appointment>>>;