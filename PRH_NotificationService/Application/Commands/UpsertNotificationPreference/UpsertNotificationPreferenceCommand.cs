using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateNotificationPreference
{
    public record UpsertNotificationPreferenceCommand(Guid UserId, Guid NotificationTypeId, bool IsSubscribed) : IRequest<BaseResponse<string>>;
}
