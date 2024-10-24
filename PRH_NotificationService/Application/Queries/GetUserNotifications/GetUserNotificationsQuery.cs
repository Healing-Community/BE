﻿using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery(string UserId, bool IncludeRead) : IRequest<BaseResponse<List<NotificationDto>>>;
}
