using System;
using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateAvailability.UpdateAvailabilityStatus;

public record UpdateAvailabilityStatusCommand(string AppointmentId, int Status) : IRequest<BaseResponse<string>>;
