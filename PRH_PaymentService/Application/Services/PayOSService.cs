using Application.Interfaces.Services;
using Domain.Contracts;
using Net.payOS;
using Net.payOS.Types;

namespace Application.Services
{
    public class PayOSService(PayOS payOS) : IPayOSService
    {
        public async Task<CreatePaymentResponse> CreatePaymentLink(PaymentRequest request)
        {
            var paymentData = new PaymentData(
                orderCode: request.OrderCode,
                amount: request.Amount,
                description: request.Description,
                items: new List<ItemData>(),
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl
            );

            var createPaymentResult = await payOS.createPaymentLink(paymentData);

            if (createPaymentResult == null)
            {
                throw new InvalidOperationException("Failed to create payment link.");
            }

            return new CreatePaymentResponse
            {
                PaymentUrl = createPaymentResult.checkoutUrl
            };
        }
    }
}
