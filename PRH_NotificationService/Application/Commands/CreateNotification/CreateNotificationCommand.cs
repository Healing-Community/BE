using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Notification
{
    public record CreateNotificationCommand(Guid UserId, Guid NotificationTypeId, string Message) : IRequest<BaseResponse<string>>;
}
