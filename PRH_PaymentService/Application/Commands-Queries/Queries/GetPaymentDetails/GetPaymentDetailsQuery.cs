using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentDetails;

public record GetPaymentDetailsQuery(string PaymentId) : IRequest<BaseResponse<PaymentDetailsDTO>>;
