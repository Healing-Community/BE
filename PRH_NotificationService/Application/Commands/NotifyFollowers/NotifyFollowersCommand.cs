using Application.Commons;
using MediatR;

namespace Application.Commands.NotifyFollowers
{
    public record NotifyFollowersCommand(Guid UserId, Guid NotificationTypeId, string PostTitle) : IRequest<BaseResponse<string>>;
}
