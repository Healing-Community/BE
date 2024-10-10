using Application.Commands.ArchiveUnreadNotifications;
using Application.Commands.MarkNotificationAsRead;
using Application.Commands.Notification;
using Application.Commands.UpdateNotificationPreference;
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
    }
}
