using Application.Commons;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public record CancelPaymentLinkCommand(long OrderCode) : IRequest<BaseResponse<string>>;
}
