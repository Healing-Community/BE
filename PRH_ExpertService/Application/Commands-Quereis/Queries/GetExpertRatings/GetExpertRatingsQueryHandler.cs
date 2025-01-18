using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.RateExpert
{
    public class GetExpertRatingsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<GetExpertRatingsQuery, BaseResponse<ExpertRatingsResponseDTO>>
    {
        public async Task<BaseResponse<ExpertRatingsResponseDTO>> Handle(GetExpertRatingsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ExpertRatingsResponseDTO>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                // Lấy danh sách các cuộc hẹn của chuyên gia
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);
                var ratedAppointments = appointments
                    .Where(a => a.Rating.HasValue && a.Rating.Value > 0)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                var totalRatings = ratedAppointments.Count;

                // Tính điểm trung bình từ các cuộc hẹn đã đánh giá
                decimal averageRating = expertProfile.AverageRating;
                if (totalRatings > 0)
                {
                    averageRating = (decimal)ratedAppointments.Average(a => a.Rating.Value);
                }

                // Tạo danh sách đánh giá
                var ratings = ratedAppointments
                    .Select(a => new ExpertRatingDTO
                    {
                        UserId = a.UserId,
                        Rating = a.Rating.Value,
                        Comment = a.Comment,
                        Time = a.UpdatedAt
                    }).ToList();

                // Tạo phản hồi dữ liệu
                var expertRatingsResponse = new ExpertRatingsResponseDTO
                {
                    AverageRating = averageRating,
                    Ratings = ratings
                };

                response.Success = true;
                response.Data = expertRatingsResponse;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách đánh giá thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách đánh giá.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
