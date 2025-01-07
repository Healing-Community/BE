using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;
using NUlid;

namespace Application.Queries.GetRecentRatings
{
    public class GetRecentRatingsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetRecentRatingsQuery, BaseResponse<IEnumerable<RecentRatingDTO>>>
    {
        public async Task<BaseResponse<IEnumerable<RecentRatingDTO>>> Handle(GetRecentRatingsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<RecentRatingDTO>>
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

                var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfileId);
                var recentRatings = appointments
                    .Where(a => a.Rating.HasValue && a.Rating.Value > 0 && a.UpdatedAt >= oneMonthAgo)
                    .Select(a => new RecentRatingDTO
                    {
                        UserId = a.UserId,
                        UserEmail = a.UserEmail, 
                        Rating = a.Rating.Value,
                        Comment = a.Comment
                    })
                    .ToList();

                response.Success = true;
                response.Data = recentRatings;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Lấy đánh giá gần đây thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy đánh giá gần đây.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
