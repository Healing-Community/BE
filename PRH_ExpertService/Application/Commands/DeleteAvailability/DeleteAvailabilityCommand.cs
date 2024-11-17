using MediatR;
using Application.Commons;

namespace Application.Commands.DeleteAvailability
{
    public record DeleteAvailabilityCommand(string AvailabilityId) : IRequest<BaseResponse<bool>>;
}
