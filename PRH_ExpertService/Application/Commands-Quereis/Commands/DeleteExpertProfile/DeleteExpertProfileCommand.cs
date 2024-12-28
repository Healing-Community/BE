using Application.Commons;
using MediatR;

namespace Application.Commands.DeleteExpertProfile
{
    public record DeleteExpertProfileCommand(string ExpertProfileId) : IRequest<BaseResponse<bool>>;
}
