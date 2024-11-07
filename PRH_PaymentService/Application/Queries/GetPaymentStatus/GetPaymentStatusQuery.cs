using Application.Commons;
using Domain.Contracts;
using MediatR;

namespace Application.Queries.GetPaymentStatus
{
    public record GetPaymentStatusQuery(long OrderCode) : IRequest<BaseResponse<PaymentStatusResponse>>;
}
