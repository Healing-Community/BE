using Application.Commons;
using MediatR;

namespace Application.Commands.BookAppointment
{
    public record BookAppointmentCommand(string ExpertAvailabilityId, string UserId) : IRequest<BaseResponse<string>>;
}
