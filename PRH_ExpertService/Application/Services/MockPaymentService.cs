using Application.Commons.Payment;
using Application.Interfaces.Services;

namespace Application.Services
{
    public class MockPaymentService : IPaymentService
    {
        // Giả lập tạo yêu cầu thanh toán
        public async Task<PaymentRequestResult> CreatePaymentRequestAsync(string paymentId, decimal amount, string userId, string qrCodeLink)
        {
            // Giả lập quá trình tạo yêu cầu thanh toán
            await Task.Delay(1000); // Giả lập thời gian xử lý 1 giây

            // Trả về kết quả giả lập thành công
            return new PaymentRequestResult
            {
                Success = true,
                PaymentId = paymentId,
                QrCodeLink = qrCodeLink,
                Message = "Yêu cầu thanh toán đã được tạo thành công."
            };
        }

        // Giả lập xác minh thanh toán
        public async Task<PaymentVerificationResult> VerifyPaymentAsync(string paymentId)
        {
            // Giả lập thời gian xử lý
            await Task.Delay(1000);

            // Giả lập xác minh thanh toán luôn thành công
            return new PaymentVerificationResult
            {
                Success = true,
                PaymentId = paymentId,
                Message = "Thanh toán đã được xác minh thành công."
            };
        }
    }
}