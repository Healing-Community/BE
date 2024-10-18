using Application.Commons;
using MediatR;

namespace Application.Commands.CreateNotification
{
    public record CreateNotificationCommand(Guid UserId, Guid NotificationTypeId, string Message) : IRequest<BaseResponse<string>>;
}
