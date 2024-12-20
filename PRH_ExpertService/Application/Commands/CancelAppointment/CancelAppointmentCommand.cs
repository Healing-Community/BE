using MediatR;
using Application.Commons;

namespace Application.Commands.CancelAppointment
{
    public record CancelAppointmentCommand(string AppointmentId) : IRequest<BaseResponse<bool>>;
}