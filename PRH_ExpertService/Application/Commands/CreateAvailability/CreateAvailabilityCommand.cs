using Application.Commons;
using MediatR;

namespace Application.Commands.CreateAvailability
{
    public record CreateAvailabilityCommand(DateTime AvailableDate, TimeSpan StartTime, TimeSpan EndTime) : IRequest<DetailBaseResponse<string>>;
}
