using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Commands.GetPaymentInfo;

public record GetPaymentInfoQuery : IRequest<BaseResponse<PaymentInfo>>;