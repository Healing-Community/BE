using Application.Commands.ArchiveUnreadNotifications;
using Application.Commands.GetPopularNotificationTypes;
using Application.Commands.GetReadNotificationRate;
using Application.Commands.MarkNotificationAsRead;
using Application.Commands.Notification;
using Application.Commands.NotifyFollowers;
using Application.Commands.UpdateNotificationPreference;
using MassTransit.Mediator;
using MediatR;
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

        [HttpPost("create-notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }

        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var response = await _sender.Send(new MarkNotificationAsReadCommand(notificationId));
            return response.ToActionResult();
        }

        [HttpPost("archive-unread/{userId}")]
        public async Task<IActionResult> ArchiveUnread(Guid userId)
        {
            var response = await _sender.Send(new ArchiveUnreadNotificationsCommand(userId));
            return response.ToActionResult();
        }

        [HttpPost("upserst-notification-preference")]
        public async Task<IActionResult> UpdateNotificationPreference([FromBody] UpsertNotificationPreferenceCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }

        [HttpPost("notify-followers")]
        public async Task<IActionResult> NotifyFollowers([FromBody] NotifyFollowersCommand command)
        {
            var response = await _sender.Send(command);
            return response.ToActionResult();
        }

        [HttpGet("read-rate")]
        public async Task<IActionResult> GetReadNotificationRate()
        {
            var response = await _sender.Send(new GetReadNotificationRateCommand());
            return response.ToActionResult();
        }

        [HttpGet("popular-types")]
        public async Task<IActionResult> GetPopularNotificationTypes()
        {
            var response = await _sender.Send(new GetPopularNotificationTypesCommand());
            return response.ToActionResult();
        }
    }
}
