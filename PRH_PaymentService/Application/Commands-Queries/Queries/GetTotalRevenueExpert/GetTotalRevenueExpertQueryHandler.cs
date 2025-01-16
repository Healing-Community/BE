using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enum;
using ExpertPaymentService;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Commands_Queries.Queries.GetTotalRevenueExpert
{
    public class GetTotalRevenueExpertQueryHandler(IPaymentRepository paymentRepository, IGrpcHelper grpcHelper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetTotalRevenueExpertQuery, BaseResponse<decimal>>
    {
        public async Task<BaseResponse<decimal>> Handle(GetTotalRevenueExpertQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy ExpertId từ token
                var expertId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Gọi gRPC để lấy danh sách lịch hẹn của Expert
                var appointmentsDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsByExpertRequest, GetAppointmentsListResponse>(
                    "ExpertServiceUrl",
                    async client => await client.GetAppointmentsByExpertAsync(new GetAppointmentsByExpertRequest { ExpertId = expertId })
                );

                var appointments = appointmentsDataReply?.Appointments.ToList();
                if (appointments == null || !appointments.Any())
                {
                    return BaseResponse<decimal>.SuccessReturn(0, "Không tìm thấy thông tin lịch hẹn.");
                }

                // Lấy danh sách AppointmentIds
                var appointmentIds = appointments.Select(a => a.AppointmentId).Distinct().ToList();

                // Lấy danh sách thanh toán dựa trên AppointmentIds
                var payments = await paymentRepository.GetsByPropertyAsync(p => appointmentIds.Contains(p.AppointmentId) && p.Status == (int)PaymentStatus.Completed);
                if (payments == null || !payments.Any())
                {
                    return BaseResponse<decimal>.SuccessReturn(0, "Không tìm thấy thông tin thanh toán.");
                }

                // Tính tổng doanh thu
                var totalRevenue = payments.Sum(p => p.ExpertAmount);

                return BaseResponse<decimal>.SuccessReturn(totalRevenue);
            }
            catch (Exception ex)
            {
                return BaseResponse<decimal>.InternalServerError(ex.Message);
            }
        }
    }
}
