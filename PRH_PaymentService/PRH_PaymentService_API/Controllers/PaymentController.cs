﻿using Application.Commands.CancelPaymentLink;
using Application.Commands.CreatePayment;
using Application.Queries.GetPaymentStatus;
using Application.Queries.GetTransactionHistory;
using Domain.Contracts;
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
        public async Task<IActionResult> CreatePayment(PaymentRequest paymentRequest)
        {
            var response = await sender.Send(new CreatePaymentCommand(paymentRequest));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            var response = await sender.Send(new GetPaymentStatusQuery(orderCode));
            return response.ToActionResult();
        }

        [HttpGet("redirect/{orderCode}/{isCancel}/{appointmentId}")]
        public async Task<IActionResult> CancelPayment(long orderCode, bool isCancel,string appointmentId, string redirectUrl)
        {
            string status = isCancel ? "cancelled" : "paid";
            if (isCancel)
            {
                await sender.Send(new UpdatePaymentStatusCommand(orderCode, (int)Domain.Enum.PaymentStatus.Cancelled, appointmentId));
            }
            else
            {
                await sender.Send(new UpdatePaymentStatusCommand(orderCode, (int)Domain.Enum.PaymentStatus.Paid, appointmentId));
            }
            return Redirect($"{redirectUrl}?status={status}");
        }

        [Authorize(Roles = "User")]
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            var response = await sender.Send(new GetTransactionHistoryQuery());
            return response.ToActionResult();
        }

        // [Authorize(Roles = "User")]
        // [HttpGet("details/{paymentId}")]
        // public async Task<IActionResult> GetPaymentDetails([FromRoute] string paymentId)
        // {
        //     var response = await sender.Send(new GetPaymentDetailsQuery(paymentId));
        //     return Ok(response);
        // }
    }
}
