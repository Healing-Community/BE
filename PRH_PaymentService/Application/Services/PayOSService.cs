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
                throw new InvalidOperationException("Không tạo được liên kết thanh toán.");
            }

            return new CreatePaymentResponse
            {
                PaymentUrl = createPaymentResult.checkoutUrl
            };
        }

        public async Task<PaymentStatusResponse> GetPaymentStatus(long orderCode)
        {
            var paymentLinkInformation = await payOS.getPaymentLinkInformation(orderCode);

            if (paymentLinkInformation == null)
            {
                throw new InvalidOperationException("Không thể lấy thông tin trạng thái thanh toán.");
            }

            return new PaymentStatusResponse
            {
                Status = paymentLinkInformation.status,
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
                throw new InvalidOperationException("Không thể hủy liên kết thanh toán.");
            }

            return new PaymentStatusResponse
            {
                Status = cancelledPaymentLinkInfo.status
            };
        }    }
}
