using Application.Commons;
using MediatR;

namespace Application.Commands.CreateAvailability
{
    public record CreateAvailabilityCommand(DateOnly AvailableDate, TimeOnly StartTime, TimeOnly EndTime) : IRequest<DetailBaseResponse<string>>;
}
