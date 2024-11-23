using Microsoft.AspNetCore.Mvc;
using PRH_PaymentService_API.Services;
using ExpertService.gRPC;

namespace PRH_PaymentService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestGrpcController(ExpertServiceGrpcClient expertServiceGrpcClient) : ControllerBase
    {
        [HttpGet("test-payment-success")]
        public async Task<IActionResult> TestPaymentSuccess()
        {
            // Tạo yêu cầu PaymentSuccessRequest thử nghiệm
            var request = new PaymentSuccessRequest
            {
                AppointmentId = "test-appointment-id",
                PaymentId = "test-payment-id"
            };

            // Gọi Expert Service qua gRPC
            var response = await expertServiceGrpcClient.PaymentSuccessAsync(request);

            // Trả về kết quả từ gRPC response
            return Ok(new
            {
                success = response.Success,
                message = response.Message
            });
        }
    }
}