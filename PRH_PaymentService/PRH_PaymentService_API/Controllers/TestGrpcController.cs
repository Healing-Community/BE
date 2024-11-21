using Microsoft.AspNetCore.Mvc;
using PRH_PaymentService_API.Services;
using ExpertService.gRPC;
using System.Threading.Tasks;

namespace PRH_PaymentService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestGrpcController : ControllerBase
    {
        private readonly ExpertServiceGrpcClient _expertServiceGrpcClient;

        public TestGrpcController(ExpertServiceGrpcClient expertServiceGrpcClient)
        {
            _expertServiceGrpcClient = expertServiceGrpcClient;
        }

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
            var response = await _expertServiceGrpcClient.PaymentSuccessAsync(request);

            // Trả về kết quả từ gRPC response
            return Ok(new
            {
                success = response.Success,
                message = response.Message
            });
        }
    }
}
