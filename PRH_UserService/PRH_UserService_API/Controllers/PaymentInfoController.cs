using Application.Commands_Queries.Commands.PaymentInfos.CreatePaymentInfo;
using Application.Commands_Queries.Commands.PaymentInfos.DeletePaymentInfo;
using Application.Commands_Queries.Commands.PaymentInfos.UpdatePaymentInfo;
using Application.Commands_Queries.Queries.GetPaymentInfo;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentInfoController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-payment-info")]
        public async Task<IActionResult> GetPaymentInfo()
        {
            var response = await sender.Send(new GetPaymentInfoQuery());
            return response.ToActionResult();
        }
        /// <summary>
        /// This method is obsolete. Use UpdatePaymentInfo instead.
        /// </summary>
        /// <param name="paymentInfoDto"></param>
        /// <returns></returns>
        [Obsolete]
        [Authorize]
        [HttpPost("create-payment-info")]
        public async Task<IActionResult> CreatePaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            var response = await sender.Send(new CreatePaymentInfoCommand(paymentInfoDto));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPut("update-payment-info")]
        public async Task<IActionResult> UpdatePaymentInfo(PaymentInfoDto paymentInfoDto)
        {
            var response = await sender.Send(new UpdatePaymentInfoCommand(paymentInfoDto));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpDelete("delete-payment-info")]
        public async Task<IActionResult> DeletePaymentInfo()
        {
            var response = await sender.Send(new DeletePaymentInfoCommand());
            return response.ToActionResult();
        }
    }
}
