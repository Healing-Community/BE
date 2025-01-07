using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateAppointment
{
    public record UpdateAppointmentCommand(string AppointmentId, DateOnly NewAppointmentDate, TimeOnly NewStartTime, TimeOnly NewEndTime) : IRequest<BaseResponse<bool>>;
}
