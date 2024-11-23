using MediatR;
using Application.Commons;

namespace Application.Commands.DeleteAvailability
{
    public record DeleteAvailabilityCommand(string expertAvailabilityId) : IRequest<BaseResponse<bool>>;
}
