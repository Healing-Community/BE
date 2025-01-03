using Application.Commands_Queries.Commands.Webhook.Handlewebhook;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_PaymentService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HookController(ISender sender) : ControllerBase
    {
        [HttpPost("handle")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookRequest request)
        {
            var response = await sender.Send(new HandleWebhookCommand(request));
            return response.ToActionResult();
        }
    }
}
