using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Domain.Enum;
using MediatR;

namespace Application.Commands_Queries.Queries.GetRevenueStatistics
{
    public class GetRevenueStatisticsQueryHandler(IPlatformFeeRepository platformFeeRepository, IPaymentRepository paymentRepository) : IRequestHandler<GetRevenueStatisticsQuery, BaseResponse<RevenueStatisticsDto>>
    {
        public async Task<BaseResponse<RevenueStatisticsDto>> Handle(GetRevenueStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get platform fee
                var platformFee = await platformFeeRepository.GetByPropertyAsync(p => p.PlatformFeeName == "PlatformFee");
                if (platformFee == null)
                {
                    return BaseResponse<RevenueStatisticsDto>.NotFound("Không tìm thấy thông tin phí.");
                }

                // Get all completed payments
                var payments = await paymentRepository.GetsByPropertyAsync(p => p.Status == (int)PaymentStatus.Completed);
                if (payments == null || !payments.Any())
                {
                    return BaseResponse<RevenueStatisticsDto>.SuccessReturn(new RevenueStatisticsDto(), "Không tìm thấy thông tin thanh toán.");
                }

                // Calculate total revenue
                var totalRevenue = payments.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100);

                // Calculate revenue for the current month
                var currentMonth = DateTime.UtcNow.Month;
                var currentYear = DateTime.UtcNow.Year;
                var currentMonthRevenue = payments
                    .Where(p => p.PaymentDate.Month == currentMonth && p.PaymentDate.Year == currentYear)
                    .Sum(p => p.Amount * platformFee.PlatformFeeValue / 100);

                var revenueStatistics = new RevenueStatisticsDto
                {
                    TotalRevenue = totalRevenue,
                    CurrentMonthRevenue = currentMonthRevenue
                };

                return BaseResponse<RevenueStatisticsDto>.SuccessReturn(revenueStatistics);
            }
            catch (Exception e)
            {
                return BaseResponse<RevenueStatisticsDto>.InternalServerError(e.Message);
            }
        }
    }
}
