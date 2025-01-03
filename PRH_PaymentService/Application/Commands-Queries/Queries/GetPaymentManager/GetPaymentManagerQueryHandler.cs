using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentManager;

public class GetPaymentManagerQueryHandler(IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentManagerQuery, BaseResponse<IEnumerable<PaymentManagerDto>>>
{
    public async Task<BaseResponse<IEnumerable<PaymentManagerDto>>> Handle(GetPaymentManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var payments = await paymentRepository.GetsAsync();
            if (payments == null)
            {
                return BaseResponse<IEnumerable<PaymentManagerDto>>.NotFound("Không tìm thấy thông tin thanh toán.");
            }
            var paymentManagerDtos = payments.Select(p => new PaymentManagerDto
            {
                AppointmentId = p.AppointmentId,
                UserId = p.UserId,
                ExpertPaymentQrCodeLink = p.ExpertPaymentQrCodeLink,
                PaymentDate = p.PaymentDate,
                PaymentDetail = p.PaymentDetail,
                UserPaymentQrCodeLink = p.UserPaymentQrCodeLink,
                PaymentId = p.PaymentId,
                OrderCode = p.OrderCode,
                Amount = p.Amount,
                Status = p.Status,
                UpdatedAt = p.UpdatedAt
            });
            return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn(paymentManagerDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
