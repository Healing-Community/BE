using Application.Commons;
using MediatR;

namespace Application.Commands.CreateAvailability
{
    public record CreateAvailabilityCommand(string ExpertProfileId, DateTime AvailableDate, TimeSpan StartTime, TimeSpan EndTime) : IRequest<BaseResponse<string>>;
}
