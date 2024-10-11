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

namespace PRH_NotificationService_API.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class notificationController : ControllerBase
    {
        private readonly ISender _sender;

        public notificationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create-notification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            var response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpPost("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var response = await _sender.Send(new MarkNotificationAsReadCommand(notificationId));
            return Ok(response);
        }

        [HttpPost("archive-unread/{userId}")]
        public async Task<IActionResult> ArchiveUnread(Guid userId)
        {
            var response = await _sender.Send(new ArchiveUnreadNotificationsCommand(userId));
            return Ok(response);
        }

        [HttpPost("upserst-notification-preference")]
        public async Task<IActionResult> UpdateNotificationPreference([FromBody] UpsertNotificationPreferenceCommand command)
        {
            var response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpPost("notify-followers")]
        public async Task<IActionResult> NotifyFollowers([FromBody] NotifyFollowersCommand command)
        {
            var response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpGet("read-rate")]
        public async Task<IActionResult> GetReadNotificationRate()
        {
            var response = await _sender.Send(new GetReadNotificationRateCommand());
            return Ok(response);
        }

        [HttpGet("popular-types")]
        public async Task<IActionResult> GetPopularNotificationTypes()
        {
            var response = await _sender.Send(new GetPopularNotificationTypesCommand());
            return Ok(response);
        }
    }
}
