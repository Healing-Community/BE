using Application.Commons;
using MediatR;

namespace Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(string AppointmentId,int Amount, string Description, string ReturnUrl, string CancelUrl) : IRequest<BaseResponse<string>>;
}
