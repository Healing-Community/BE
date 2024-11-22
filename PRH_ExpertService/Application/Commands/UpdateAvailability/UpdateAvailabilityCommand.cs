using MediatR;
using Application.Commons;

namespace Application.Commands.UpdateAvailability
{
    public record UpdateAvailabilityCommand(string expertAvailabilityId, DateTime NewAvailableDate, TimeSpan NewStartTime, TimeSpan NewEndTime) : IRequest<DetailBaseResponse<bool>>;
}
