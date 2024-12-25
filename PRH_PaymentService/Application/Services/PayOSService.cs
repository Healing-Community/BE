using Application.Interfaces.Services;
using Domain.Contracts;
using Net.payOS;
using Net.payOS.Types;

namespace Application.Services
{
    public class PayOSService(PayOS payOS) : IPayOSService
    {
        private readonly PayOS _payOS = payOS ?? throw new ArgumentNullException(nameof(payOS));

        public async Task<CreatePaymentResponse> CreatePaymentLink(PaymentRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var paymentData = new PaymentData(
                orderCode: request.OrderCode,
                amount: request.Amount,
                description: request.Description,
                items: [],
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl
            );

            var createPaymentResult = await _payOS.createPaymentLink(paymentData);

            if (createPaymentResult == null)
                throw new InvalidOperationException("Unable to create payment link.");

            return new CreatePaymentResponse
            {
                PaymentUrl = createPaymentResult.checkoutUrl,
                Status = createPaymentResult.status
            };
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(long orderCode)
        {
            var paymentLinkInfo = await _payOS.getPaymentLinkInformation(orderCode)
                                   ?? throw new InvalidOperationException("Unable to fetch payment status.");

            return new PaymentStatusResponse
            {
                Status = paymentLinkInfo.status
            };
        }

        public async Task<PaymentStatusResponse> CancelPaymentLink(long orderCode, string? reason = null)
        {
            var cancelledLinkInfo = string.IsNullOrEmpty(reason)
                ? await _payOS.cancelPaymentLink(orderCode)
                : await _payOS.cancelPaymentLink(orderCode, reason);

            if (cancelledLinkInfo == null)
                throw new InvalidOperationException("Unable to cancel payment link.");

            return new PaymentStatusResponse
            {
                Status = cancelledLinkInfo.status
            };
        }
    }
}
