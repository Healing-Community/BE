using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands.CancelPaymentLink
{
    public class CancelPaymentLinkCommandHandler(IPayOSService payOSService, IPaymentRepository paymentRepository) : IRequestHandler<CancelPaymentLinkCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CancelPaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // Hủy liên kết thanh toán trên cổng thanh toán PayOS
                var cancelResult = await payOSService.CancelPaymentLink(request.ReturnPaymentDto.OrderCode);
                // Cập nhật trạng thái hủy liên kết thanh toán trong cơ sở dữ liệu
                var payment = await paymentRepository.GetByPropertyAsync(p => p.OrderCode == request.ReturnPaymentDto.OrderCode);
                await paymentRepository.Update(payment.PaymentId, new Payment
                {
                    UserId = payment.UserId,
                    AppointmentId = payment.AppointmentId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    UpdatedAt = DateTime.UtcNow,
                    PaymentId = payment.PaymentId,
                    OrderCode = payment.OrderCode,
                    Status = 3
                });
                response.Success = true;
                response.Data = cancelResult.Status;
                response.StatusCode = 200;
                response.Message = "Hủy liên kết thanh toán thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Hủy liên kết thanh toán thất bại.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
