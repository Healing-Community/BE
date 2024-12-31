using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.PaymentInfos.CreatePaymentInfo;

public record CreatePaymentInfoCommand(PaymentInfoDto PaymentInfoDto) : IRequest<BaseResponse<string>>;

