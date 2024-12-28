using MediatR;
using Application.Commons;

namespace Application.Commands.DeleteAppointment
{
    public record DeleteAppointmentCommand(string AppointmentId) : IRequest<BaseResponse<bool>>;
}
