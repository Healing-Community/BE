using Application.Commons.Payment;

namespace Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentRequestResult> CreatePaymentRequestAsync(string paymentId, decimal amount, string userId, string qrCodeLink);
        Task<PaymentVerificationResult> VerifyPaymentAsync(string paymentId);
    }
}