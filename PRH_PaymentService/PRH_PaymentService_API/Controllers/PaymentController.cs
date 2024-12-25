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
    public class PaymentController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(PaymentPayloadDto paymentPayloadDto)
        {
            long orderCode = 123;
            var cancelUrl = Url.Action("cancel", "Payment", new ReturnPaymentDto { OrderCode = orderCode, RedirectUrl = paymentPayloadDto.ReturnUrl }, protocol: Request.Scheme);
            var returnUrl = Url.Action("return", "Payment", new ReturnPaymentDto { OrderCode = orderCode, RedirectUrl = paymentPayloadDto.ReturnUrl }, protocol: Request.Scheme);
            
            paymentPayloadDto.OrderCode = orderCode;
            var response = await sender.Send(new CreatePaymentCommand(paymentPayloadDto));
            return Ok(response);
        }

        [Authorize]
        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            var response = await sender.Send(new GetPaymentStatusQuery(orderCode));
            return Ok(response);
        }

        [Authorize]
        [HttpGet("cancel/{orderCode}")]
        public async Task<IActionResult> CancelPayment([FromQuery] ReturnPaymentDto returnPaymentDto)
        {
            // Huỷ thanh toán
            await sender.Send(new CancelPaymentLinkCommand(returnPaymentDto));
            // Redirect về trang gốc
            return Redirect($"{returnPaymentDto.RedirectUrl}?status=CANCEL");
        }
        [Authorize]
        [HttpGet("return/{orderCode}")]
        public async Task<IActionResult> ReturnPayment([FromQuery] ReturnPaymentDto returnPaymentDto)
        {
            await sender.Send(new CancelPaymentLinkCommand(returnPaymentDto));
            return Redirect($"{returnPaymentDto.RedirectUrl}?status=SUCCEED");
        }
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            var response = await sender.Send(new GetTransactionHistoryQuery());
            return Ok(response);
        }

        [Authorize]
        [HttpGet("details/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetails([FromRoute] string paymentId)
        {
            var response = await sender.Send(new GetPaymentDetailsQuery(paymentId));
            return Ok(response);
        }
    }
}
