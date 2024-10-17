using Application.Commons;
using MediatR;

namespace Application.Commands.NotifyFollowers
{
    public record NotifyFollowersCommand(Guid UserId, string PostTitle) : IRequest<BaseResponse<string>>;
}
