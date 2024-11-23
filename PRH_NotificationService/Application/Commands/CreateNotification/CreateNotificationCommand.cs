using Application.Commons;
using MediatR;

namespace Application.Commands.CreateNotification
{
    public record CreateNotificationCommand(string NotificationTypeId, string Message) : IRequest<BaseResponse<string>>;
}
