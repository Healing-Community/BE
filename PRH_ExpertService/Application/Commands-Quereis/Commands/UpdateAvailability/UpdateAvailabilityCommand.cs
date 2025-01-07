using MediatR;
using Application.Commons;

namespace Application.Commands.UpdateAvailability
{
    public record UpdateAvailabilityCommand(string ExpertAvailabilityId, DateOnly NewAvailableDate, TimeOnly NewStartTime, TimeOnly NewEndTime) : IRequest<BaseResponse<bool>>;
}
