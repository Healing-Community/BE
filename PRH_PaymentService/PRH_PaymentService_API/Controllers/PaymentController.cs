using Application.Commands.CancelPaymentLink;
using Application.Commands.CreatePayment;
using Application.Queries.GetPaymentStatus;
using Application.Queries.GetTransactionHistory;
using Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PaymentService_API.Services;

namespace PRH_PaymentService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(ISender sender, PaymentDbContext paymentDbContext) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(CreatePaymentCommand command)
        {
            var response = await sender.Send(command);
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            var response = await sender.Send(new GetPaymentStatusQuery(orderCode));
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentLinkCommand command)
        {
            var response = await sender.Send(command);
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            var response = await sender.Send(new GetTransactionHistoryQuery());
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("details/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetails([FromRoute] string paymentId)
        {
            var response = await sender.Send(new GetPaymentDetailsQuery(paymentId));
            return Ok(response);
        }

    }
}
