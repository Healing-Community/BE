using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.PaymentInfos.DeletePaymentInfo;

public record DeletePaymentInfoCommand : IRequest<BaseResponse<string>>;
