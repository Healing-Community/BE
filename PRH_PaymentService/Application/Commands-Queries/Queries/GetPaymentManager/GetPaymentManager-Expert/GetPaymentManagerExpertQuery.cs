using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_Expert;


public record GetPaymentManagerExpertQuery : IRequest<BaseResponse<IEnumerable<PaymentManagerUserDto>>>;