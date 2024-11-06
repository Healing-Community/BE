using Application.Commands.CreatePayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_PaymentService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(CreatePaymentCommand command)
        {
            var response = await sender.Send(command);
            return Ok(response);
        }
    }
}