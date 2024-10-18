using Application.Commons;
using MediatR;

namespace Application.Commands.ArchiveUnreadNotifications
{
    public record ArchiveUnreadNotificationsCommand(Guid UserId) : IRequest<BaseResponse<string>>;
}
