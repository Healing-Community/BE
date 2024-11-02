using MediatR;
using Application.Commons;

namespace Application.Commands.UpdateAvailability
{
    public record UpdateAvailabilityCommand(string AvailabilityId, DateTime NewAvailableDate, TimeSpan NewStartTime, TimeSpan NewEndTime) : IRequest<DetailBaseResponse<bool>>;
}
