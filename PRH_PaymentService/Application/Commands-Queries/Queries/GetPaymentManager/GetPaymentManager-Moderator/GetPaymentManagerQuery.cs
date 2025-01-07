using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentManager;

public record GetPaymentManagerQuery : IRequest<BaseResponse<IEnumerable<PaymentManagerDto>>>;