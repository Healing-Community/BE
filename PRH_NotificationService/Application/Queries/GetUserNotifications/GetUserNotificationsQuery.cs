using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery() : IRequest<BaseResponse<List<NotificationDto>>>;
}
