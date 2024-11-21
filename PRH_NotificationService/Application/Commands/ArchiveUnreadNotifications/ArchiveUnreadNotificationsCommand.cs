using Application.Commons;
using MediatR;

namespace Application.Commands.ArchiveUnreadNotifications
{
    public record ArchiveUnreadNotificationsCommand() : IRequest<BaseResponse<string>>;
}
