using Domain.Contracts;

namespace Application.Interfaces.Services
{
    public interface IPayOSService
    {
        Task<CreatePaymentResponse> CreatePaymentLink(PaymentRequest request);
    }
}