using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;
using NUlid;

namespace Application.Queries.GetActivityReport
{
    public class GetActivityReportQueryHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetActivityReportQuery, BaseResponse<IEnumerable<ActivityReportDTO>>>
    {
        public async Task<BaseResponse<IEnumerable<ActivityReportDTO>>> Handle(GetActivityReportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ActivityReportDTO>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy ExpertProfileId từ token
                var expertProfileId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (string.IsNullOrEmpty(expertProfileId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác thực người dùng.";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfileId);
                var groupedAppointments = appointments
                    .GroupBy(a => a.AppointmentDate)
                    .Select(g => new ActivityReportDTO
                    {
                        Date = g.Key,
                        TotalAppointments = g.Count(),
                        CompletedAppointments = g.Count(a => a.Status == 3),
                        CompletionRate = g.Count(a => a.Status == 3) / (double)g.Count() * 100,
                        Status = GetStatus(g.Count(a => a.Status == 3) / (double)g.Count() * 100)
                    })
                    .ToList();

                response.Success = true;
                response.Data = groupedAppointments;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Lấy báo cáo hoạt động thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy báo cáo hoạt động.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }

        private string GetStatus(double completionRate)
        {
            if (completionRate == 100)
            {
                return "Hoàn thành";
            }
            else if (completionRate >= 50)
            {
                return "Gần hoàn thành";
            }
            else
            {
                return "Chưa hoàn thành";
            }
        }
    }
}
