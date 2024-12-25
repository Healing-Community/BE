using Application.Commons;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public record CancelPaymentLinkCommand(ReturnPaymentDto ReturnPaymentDto) : IRequest<BaseResponse<string>>;
}
