using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateAppointment
{
    public record UpdateAppointmentCommand(string AppointmentId, DateTime NewAppointmentDate, TimeSpan NewStartTime, TimeSpan NewEndTime) : IRequest<BaseResponse<bool>>;
}
