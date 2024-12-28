using System;
using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateAppointmentStatus;

public record UpdateAppointmentStatusCommand(string AppointmentId, int Status) : IRequest<BaseResponse<string>>;
