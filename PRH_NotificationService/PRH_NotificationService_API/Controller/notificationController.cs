using Application.Commands.Notification;
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
    }
}
