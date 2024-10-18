﻿using Application.Commands.ArchiveUnreadNotifications;
using Application.Commands.CreateNotificationType;
using Application.Commands.DeleteNotification;
using Application.Commands.MarkNotificationAsRead;
using Application.Commands.Notification;
using Application.Commands.UpdateNotificationPreference;
using Application.Queries.GetPopularNotificationTypes;
using Application.Queries.GetReadNotificationRate;
using Application.Queries.GetUnreadNotificationCount;
using Application.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_NotificationService_API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationController : ControllerBase
    {
        private readonly ISender _sender;

        public NotificationController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize]
        [HttpPost("create-notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var response = await _sender.Send(new MarkNotificationAsReadCommand(notificationId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("archive-unread/{userId}")]
        public async Task<IActionResult> ArchiveUnread(Guid userId)
        {
            var response = await _sender.Send(new ArchiveUnreadNotificationsCommand(userId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("upserst-notification-preference")]
        public async Task<IActionResult> UpdateNotificationPreference([FromBody] UpsertNotificationPreferenceCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("read-rate")]
        public async Task<IActionResult> GetReadNotificationRate()
        {
            var response = await _sender.Send(new GetReadNotificationRateQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("popular-types")]
        public async Task<IActionResult> GetPopularNotificationTypes()
        {
            var response = await _sender.Send(new GetPopularNotificationTypesQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadNotificationCount(Guid userId)
        {
            var response = await _sender.Send(new GetUnreadNotificationCountQuery(userId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] bool includeRead = false)
        {
            var response = await _sender.Send(new GetUserNotificationsQuery(userId, includeRead));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpDelete("delete/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var response = await _sender.Send(new DeleteNotificationCommand(notificationId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("create-notification-type")]
        public async Task<IActionResult> CreateNotificationType([FromBody] CreateNotificationTypeCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }
    }
}
