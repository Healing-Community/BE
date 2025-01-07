using Application.Commons;
using MediatR;

namespace Application.Commands.ApproveExpertProfile
{
    public record ApproveExpertProfileCommand(string ExpertProfileId) : IRequest<BaseResponse<bool>>;
}