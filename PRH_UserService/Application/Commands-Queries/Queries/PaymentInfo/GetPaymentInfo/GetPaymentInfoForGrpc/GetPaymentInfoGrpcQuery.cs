using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentInfo.GetPaymentInfoForGrpc;

public record GetPaymentInfoGrpcQuery(string UserId) : IRequest<BaseResponse<PaymentInfo>>;