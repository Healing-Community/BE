using Application.Commons;
using MediatR;

namespace Application.Commands.ArchiveUnreadNotifications
{
    public record ArchiveUnreadNotificationsCommand(string UserId) : IRequest<BaseResponse<string>>;
}
