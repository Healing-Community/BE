using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Domain.Enum;
using MediatR;

namespace Application.Commands_Queries.Queries.GetRevenueStatistics
{
    public class GetRevenueStatisticsQueryHandler(IPlatformFeeRepository platformFeeRepository, IPaymentRepository paymentRepository) : IRequestHandler<GetRevenueStatisticsQuery, BaseResponse<IEnumerable<RevenueStatisticsDto>>>
    {
        public async Task<BaseResponse<IEnumerable<RevenueStatisticsDto>>> Handle(GetRevenueStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy thông tin phí nền tảng
                var platformFee = await platformFeeRepository.GetByPropertyAsync(p => p.PlatformFeeName == "PlatformFee");
                if (platformFee == null)
                {
                    return BaseResponse<IEnumerable<RevenueStatisticsDto>>.NotFound("Không tìm thấy thông tin phí.");
                }

                // Lấy danh sách thanh toán đã hoàn thành
                var payments = await paymentRepository.GetsByPropertyAsync(p => p.Status == (int)PaymentStatus.Completed);
                if (payments == null || !payments.Any())
                {
                    return BaseResponse<IEnumerable<RevenueStatisticsDto>>.SuccessReturn(new List<RevenueStatisticsDto>(), "Không tìm thấy thông tin thanh toán.");
                }

                IEnumerable<RevenueStatisticsDto> groupedStatistics;

                // Lọc theo loại nhóm (FilterType)
                switch (request.FilterType?.ToLower())
                {
                    case "day":
                        groupedStatistics = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month, p.PaymentDate.Day })
                                                    .Select(g => new RevenueStatisticsDto
                                                    {
                                                        Year = g.Key.Year,
                                                        Month = g.Key.Month,
                                                        Day = g.Key.Day,
                                                        DayOfWeek = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).DayOfWeek.ToString(),
                                                        TotalRevenue = g.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100),
                                                        TotalBookings = g.Count()
                                                    }).ToList();
                        break;

                    case "week":
                        groupedStatistics = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month, WeekOfMonth = GetWeekOfMonth(p.PaymentDate) })
                                                    .Select(g => new RevenueStatisticsDto
                                                    {
                                                        Year = g.Key.Year,
                                                        Month = g.Key.Month,
                                                        WeekOfMonth = g.Key.WeekOfMonth,
                                                        TotalRevenue = g.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100),
                                                        TotalBookings = g.Count()
                                                    }).ToList();
                        break;

                    case "month":
                        groupedStatistics = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                                                    .Select(g => new RevenueStatisticsDto
                                                    {
                                                        Year = g.Key.Year,
                                                        Month = g.Key.Month,
                                                        TotalRevenue = g.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100),
                                                        TotalBookings = g.Count()
                                                    }).ToList();
                        break;

                    case "year":
                        groupedStatistics = payments.GroupBy(p => p.PaymentDate.Year)
                                                    .Select(g => new RevenueStatisticsDto
                                                    {
                                                        Year = g.Key,
                                                        TotalRevenue = g.Sum(p => p.Amount * platformFee.PlatformFeeValue / 100),
                                                        TotalBookings = g.Count()
                                                    }).ToList();
                        break;

                    default:
                        return BaseResponse<IEnumerable<RevenueStatisticsDto>>.BadRequest("Invalid filter type.");
                }

                return BaseResponse<IEnumerable<RevenueStatisticsDto>>.SuccessReturn(groupedStatistics);
            }
            catch (Exception e)
            {
                return BaseResponse<IEnumerable<RevenueStatisticsDto>>.InternalServerError(e.Message);
            }
        }

        private static int GetWeekOfMonth(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDayWeek = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayWeek == 0) firstDayWeek = 7; // Sunday as 7

            int currentDayWeek = (int)date.DayOfWeek;
            if (currentDayWeek == 0) currentDayWeek = 7; // Sunday as 7

            return ((date.Day + firstDayWeek - 2) / 7) + 1;
        }
    }
}
