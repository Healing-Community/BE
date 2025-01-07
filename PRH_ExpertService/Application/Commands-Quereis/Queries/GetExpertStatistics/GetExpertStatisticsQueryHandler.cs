using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Queries.GetExpertStatistics
{
    public class GetExpertStatisticsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetExpertStatisticsQuery, BaseResponse<ExpertStatisticsDTO>>
    {
        public async Task<BaseResponse<ExpertStatisticsDTO>> Handle(GetExpertStatisticsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ExpertStatisticsDTO>
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

                var expertProfile = await expertProfileRepository.GetByIdAsync(expertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfileId);
                var completedAppointments = appointments.Where(a => a.Status == 3).ToList(); // Completed
                var ratedAppointments = completedAppointments.Where(a => a.Rating.HasValue && a.Rating.Value > 0).ToList();

                var totalAppointments = completedAppointments.Count;
                var averageRating = ratedAppointments.Any() ? ratedAppointments.Average(a => a.Rating.Value) : 0;

                var statistics = new ExpertStatisticsDTO
                {
                    TotalAppointments = totalAppointments,
                    AverageRating = averageRating
                };

                response.Success = true;
                response.Data = statistics;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Lấy thống kê thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thống kê.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}

