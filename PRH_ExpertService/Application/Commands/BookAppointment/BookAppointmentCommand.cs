using Application.Commons;
using MediatR;

namespace Application.Commands.BookAppointment
{
    public record BookAppointmentCommand(string ExpertAvailabilityId) : IRequest<BaseResponse<string>>;
}
