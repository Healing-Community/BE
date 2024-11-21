using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateNotificationPreference
{
    public record UpsertNotificationPreferenceCommand(string NotificationTypeId, bool IsSubscribed) : IRequest<BaseResponse<string>>;
}
