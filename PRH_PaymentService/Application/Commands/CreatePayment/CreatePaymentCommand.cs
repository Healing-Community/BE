using Application.Commons;
using MediatR;

namespace Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(PaymentPayloadDto PaymentPayloadDto) : IRequest<BaseResponse<string>>;
}
