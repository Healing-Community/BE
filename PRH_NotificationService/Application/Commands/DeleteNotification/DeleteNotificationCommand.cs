using Application.Commons;
using MediatR;

namespace Application.Commands.DeleteNotification
{
    public record DeleteNotificationCommand(string NotificationId) : IRequest<BaseResponse<string>>;
}
