using Application.Commons;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public record CancelPaymentLinkCommand(long OrderCode, string? Reason = null) : IRequest<BaseResponse<string>>;
}
