using Application.Commons;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public record UpdatePaymentStatusCommand(long OrderCode, int Status, string AppointmentId) : IRequest<BaseResponse<string>>;
}
