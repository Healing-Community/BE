using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.PaymentInfos.UpdatePaymentInfo;

public record UpdatePaymentInfoCommand(PaymentInfoDto PaymentInfoDto) : IRequest<BaseResponse<string>>;

