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
                throw new InvalidOperationException("Unable to create payment link.");
            }

            return new CreatePaymentResponse
            {
                PaymentUrl = createPaymentResult.checkoutUrl,
                Status = createPaymentResult.status
            };
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(long orderCode)
        {
            var paymentLinkInformation = await payOS.getPaymentLinkInformation(orderCode);

            if (paymentLinkInformation == null)
            {
                throw new InvalidOperationException("Unable to fetch payment status.");
            }

            return new PaymentStatusResponse
            {
                Status = paymentLinkInformation.status
            };
        }

        public async Task<PaymentStatusResponse> CancelPaymentLink(long orderCode, string? reason = null)
        {
            PaymentLinkInformation cancelledPaymentLinkInfo;

            if (string.IsNullOrEmpty(reason))
            {
                cancelledPaymentLinkInfo = await payOS.cancelPaymentLink(orderCode);
            }
            else
            {
                cancelledPaymentLinkInfo = await payOS.cancelPaymentLink(orderCode, reason);
            }

            if (cancelledPaymentLinkInfo == null)
            {
                throw new InvalidOperationException("Unable to cancel payment link.");
            }

            return new PaymentStatusResponse
            {
                Status = cancelledPaymentLinkInfo.status
            };
        }
    }
}
