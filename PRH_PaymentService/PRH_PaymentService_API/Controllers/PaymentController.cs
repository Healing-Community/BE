using Application.Commands.CancelPaymentLink;
using Application.Commands.CreatePayment;
using Application.Commands_Queries.Queries.GetExpertRevenueDetails;
using Application.Commands_Queries.Queries.GetPaymentDetails;
using Application.Commands_Queries.Queries.GetPaymentInfo;
using Application.Commands_Queries.Queries.GetPaymentManager;
using Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_Expert;
using Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_User;
using Application.Commands_Queries.Queries.GetRevenueStatistics;
using Application.Commands_Queries.Queries.GetTotalRevenueAdmin;
using Application.Commands_Queries.Queries.GetTotalRevenueExpert;
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
        public async Task<IActionResult> CancelPayment(long orderCode, bool isCancel, string appointmentId, string redirectUrl)
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

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            var response = await sender.Send(new GetTransactionHistoryQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("details/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetails([FromRoute] string paymentId)
        {
            var response = await sender.Send(new GetPaymentDetailsQuery(paymentId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Moderator")]
        [HttpGet("manager-payment-moderator")]
        public async Task<IActionResult> GetPaymentManager()
        {
            var response = await sender.Send(new GetPaymentManagerQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("get-payments-manager-expert")]
        public async Task<IActionResult> GetPaymentManagerExpert()
        {
            var response = await sender.Send(new GetPaymentManagerExpertQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-payments-manager-user")]
        public async Task<IActionResult> GetPaymentManagerUser()
        {
            var response = await sender.Send(new GetPaymentManagerUserQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("revenue-details-experts")]
        public async Task<IActionResult> GetExpertRevenueDetails([FromQuery] string filterType)
        {
            var response = await sender.Send(new GetExpertRevenueDetailsQuery
            {
                FilterType = filterType
            });
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("revenue-statistics-admin")]
        public async Task<IActionResult> GetRevenueStatistics([FromQuery] string filterType)
        {
            var response = await sender.Send(new GetRevenueStatisticsQuery
            {
                FilterType = filterType
            });
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("total-revenue-admin")]
        public async Task<IActionResult> GetTotalRevenueForAdmin()
        {
            var response = await sender.Send(new GetTotalRevenueAdminQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("total-revenue-expert")]
        public async Task<IActionResult> GetTotalRevenueForExpert()
        {
            var response = await sender.Send(new GetTotalRevenueExpertQuery());
            return response.ToActionResult();
        }
    }
}
