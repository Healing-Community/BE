using Application.Commons;
using Domain.Enum;
using MediatR;

namespace Application.Commands.Notification
{
    public record CreateNotificationCommand(Guid UserId, NotificationTypeEnum NotificationType, string Message) : IRequest<BaseResponse<string>>;
}
