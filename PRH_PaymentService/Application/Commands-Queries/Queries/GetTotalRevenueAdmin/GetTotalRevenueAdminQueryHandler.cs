using Application.Commons;
using Application.Interfaces.Repositories;
using Domain.Enum;
using MediatR;

namespace Application.Commands_Queries.Queries.GetTotalRevenueAdmin
{
    public class GetTotalRevenueAdminQueryHandler(IPaymentRepository paymentRepository, IPlatformFeeRepository platformFeeRepository) : IRequestHandler<GetTotalRevenueAdminQuery, BaseResponse<decimal>>
    {
        public async Task<BaseResponse<decimal>> Handle(GetTotalRevenueAdminQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy phí nền tảng
                var platformFee = await platformFeeRepository.GetByPropertyAsync(p => p.PlatformFeeName == "PlatformFee");
                if (platformFee == null)
                {
                    return BaseResponse<decimal>.NotFound("Không tìm thấy thông tin phí nền tảng.");
                }

                // Lấy danh sách thanh toán đã hoàn thành
                var payments = await paymentRepository.GetsByPropertyAsync(p => p.Status == (int)PaymentStatus.Completed);
                if (payments == null || !payments.Any())
                {
                    return BaseResponse<decimal>.SuccessReturn(0, "Không tìm thấy thông tin thanh toán.");
                }

                // Tính tổng doanh thu
                var totalRevenue = payments.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100);
                return BaseResponse<decimal>.SuccessReturn(totalRevenue);
            }
            catch (Exception ex)
            {
                return BaseResponse<decimal>.InternalServerError(ex.Message);
            }
        }
    }
}
