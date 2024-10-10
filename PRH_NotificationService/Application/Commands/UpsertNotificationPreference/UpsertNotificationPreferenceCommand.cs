using Application.Commons;
using Domain.Enum;
using MediatR;

namespace Application.Commands.UpdateNotificationPreference
{
    public record UpsertNotificationPreferenceCommand(Guid UserId, NotificationTypeEnum NotificationType, bool IsSubscribed) : IRequest<BaseResponse<string>>;
}
