using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentInfo;

public record GetPaymentInfoQuery : IRequest<BaseResponse<PaymentInfo>>;