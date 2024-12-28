using Application.Commons;
using Domain.Contracts;
using MediatR;

namespace Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(PaymentRequest PaymentRequest) : IRequest<BaseResponse<string>>;
}
