using Application.Commons;
using MediatR;

namespace Application.Commands.DeleteNotification
{
    public record DeleteNotificationCommand(Guid NotificationId) : IRequest<BaseResponse<string>>;
}
