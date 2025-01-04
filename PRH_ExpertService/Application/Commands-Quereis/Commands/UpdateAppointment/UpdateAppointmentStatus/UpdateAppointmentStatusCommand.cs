using System;
using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateAppointment.UpdateAppointmentStatus;

public record UpdateAppointmentStatusCommand(string AppointmentId, int Status) : IRequest<BaseResponse<string>>;
