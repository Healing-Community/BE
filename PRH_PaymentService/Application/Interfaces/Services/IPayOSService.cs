using Domain.Contracts;

namespace Application.Interfaces.Services
{
    public interface IPayOSService
    {
        Task<CreatePaymentResponse> CreatePaymentLink(PaymentRequest request);
        Task<PaymentStatusResponse> GetPaymentStatus(long orderCode);
        Task<PaymentStatusResponse> CancelPaymentLink(long orderCode, string? reason = null);
    }
}