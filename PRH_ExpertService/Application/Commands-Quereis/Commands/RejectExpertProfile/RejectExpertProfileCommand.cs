using Application.Commons;
using MediatR;

namespace Application.Commands.RejectExpertProfile
{
    public record RejectExpertProfileCommand(string ExpertProfileId) : IRequest<DetailBaseResponse<bool>>;
}
