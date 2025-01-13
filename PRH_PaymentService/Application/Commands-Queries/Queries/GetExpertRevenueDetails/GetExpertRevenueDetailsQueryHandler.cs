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

            // Group payments by month
            var groupedPayments = payments.GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                                          .Select(g => new ExpertRevenueDetailsDto
                                          {
                                              Year = g.Key.Year,
                                              Month = g.Key.Month,
                                              TotalRevenue = g.Sum(p => p.ExpertAmount),
                                              TotalBookings = g.Count()
                                          }).ToList();

            return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.SuccessReturn(groupedPayments);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>.InternalServerError(e.Message);
        }
    }
}
