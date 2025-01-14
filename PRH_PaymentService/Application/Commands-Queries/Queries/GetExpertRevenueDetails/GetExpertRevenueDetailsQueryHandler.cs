using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Domain.Enum;

namespace Application.Commands_Queries.Queries.GetExpertRevenueDetails;

public class GetExpertRevenueDetailsQueryHandler(IHttpContextAccessor accessor, IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<GetExpertRevenueDetailsQuery, BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>>
{
    public async Task<BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>> Handle(GetExpertRevenueDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var expertId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch appointments by expert
            var appointmentsDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsByExpertRequest, GetAppointmentsListResponse>(
                "ExpertServiceUrl",
                async client => await client.GetAppointmentsByExpertAsync(new GetAppointmentsByExpertRequest { ExpertId = expertId })
            );

            var appointments = appointmentsDataReply?.Appointments.ToList();

            if (appointments == null || !appointments.Any())
            {
                return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.SuccessReturn(new List<ExpertRevenueDetailsDto>(), "Không tìm thấy thông tin lịch hẹn.");
            }

            // Collect all appointmentIds for querying payments
            var appointmentIds = appointments.Select(a => a.AppointmentId).Distinct().ToList();

            // Fetch payments based on appointments and filter by status Completed
            var payments = await paymentRepository.GetsByPropertyAsync(x => appointmentIds.Contains(x.AppointmentId) && x.Status == (int)PaymentStatus.Completed);
            if (payments == null || !payments.Any())
            {
                return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.SuccessReturn(new List<ExpertRevenueDetailsDto>(), "Không tìm thấy thông tin thanh toán.");
            }

            // Convert PaymentDate to GMT+7
            foreach (var payment in payments)
            {
                payment.PaymentDate = ConvertToGmtPlus7(payment.PaymentDate);
            }

            // Group payments based on filter type
            IEnumerable<ExpertRevenueDetailsDto> groupedPayments;
            switch (request.FilterType.ToLower())
            {
                case "day":
                    groupedPayments = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month, p.PaymentDate.Day })
                                              .Select(g => new ExpertRevenueDetailsDto
                                              {
                                                  Year = g.Key.Year,
                                                  Month = g.Key.Month,
                                                  Day = g.Key.Day,
                                                  DayOfWeek = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).DayOfWeek.ToString(),
                                                  TotalRevenue = g.Sum(p => p.ExpertAmount),
                                                  TotalBookings = g.Count()
                                              }).ToList();
                    break;

                case "week":
                    groupedPayments = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month, WeekOfMonth = GetWeekOfMonth(p.PaymentDate) })
                                              .Select(g => new ExpertRevenueDetailsDto
                                              {
                                                  Year = g.Key.Year,
                                                  Month = g.Key.Month,
                                                  WeekOfMonth = g.Key.WeekOfMonth,
                                                  TotalRevenue = g.Sum(p => p.ExpertAmount),
                                                  TotalBookings = g.Count()
                                              }).ToList();
                    break;

                case "month":
                    groupedPayments = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                                              .Select(g => new ExpertRevenueDetailsDto
                                              {
                                                  Year = g.Key.Year,
                                                  Month = g.Key.Month,
                                                  TotalRevenue = g.Sum(p => p.ExpertAmount),
                                                  TotalBookings = g.Count()
                                              }).ToList();
                    break;

                case "year":
                    groupedPayments = payments.GroupBy(p => p.PaymentDate.Year)
                                              .Select(g => new ExpertRevenueDetailsDto
                                              {
                                                  Year = g.Key,
                                                  TotalRevenue = g.Sum(p => p.ExpertAmount),
                                                  TotalBookings = g.Count()
                                              }).ToList();
                    break;

                default:
                    return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.BadRequest("Invalid filter type.");
            }

            return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.SuccessReturn(groupedPayments);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.InternalServerError(e.Message);
        }
    }

    private static DateTime ConvertToGmtPlus7(DateTime utcDate)
    {
        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZoneInfo);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new Exception("The specified time zone (SE Asia Standard Time) could not be found.");
        }
        catch (InvalidTimeZoneException)
        {
            throw new Exception("The specified time zone (SE Asia Standard Time) is invalid.");
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
