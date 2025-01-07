using Application.Commons;
using MediatR;

namespace Application.Commands.CreateAvailability
{
    public record CreateAvailabilityCommand(DateOnly AvailableDate, TimeOnly StartTime, TimeOnly EndTime, int Amount) : IRequest<BaseResponse<string>>;
}
