using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Quereis.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(string AppointmentId) : IRequest<BaseResponse<Appointment>>;
