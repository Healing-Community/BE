using Application.Commons;
using MediatR;

namespace Application.Commands.RateExpert
{
    public record RateExpertCommand(string AppointmentId, int Rating, string Comment) : IRequest<BaseResponse<bool>>;
}
