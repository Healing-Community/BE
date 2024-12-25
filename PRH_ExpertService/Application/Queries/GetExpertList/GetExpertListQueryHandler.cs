using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetExpertList
{
    public class GetExpertListQueryHandler(
        IExpertProfileRepository expertProfileRepository,
        IAppointmentRepository appointmentRepository) : IRequestHandler<GetExpertListQuery, BaseResponse<IEnumerable<ExpertListDTO>>>
    {
        public async Task<BaseResponse<IEnumerable<ExpertListDTO>>> Handle(GetExpertListQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ExpertListDTO>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var expertProfiles = await expertProfileRepository.GetsAsync();
                var expertDetailsDtos = new List<ExpertListDTO>();

                foreach (var expertProfile in expertProfiles)
                {
                    // Lấy tất cả các cuộc hẹn của chuyên gia
                    var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfile.ExpertProfileId);
                    var completedAppointments = appointments.Where(a => a.Status == 3).ToList();

                    // Lọc các cuộc hẹn đã được đánh giá
                    var ratedAppointments = completedAppointments.Where(a => a.Rating.HasValue && a.Rating.Value > 0).ToList();

                    // Tính tổng số đánh giá
                    var totalRatings = ratedAppointments.Count;

                    // Tính trung bình đánh giá và làm tròn đến nửa sao gần nhất
                    decimal averageRating = 0M;
                    if (totalRatings > 0)
                    {
                        averageRating = (decimal)ratedAppointments.Average(a => a.Rating.Value);
                        averageRating = Math.Round(averageRating * 2, MidpointRounding.AwayFromZero) / 2;
                    }

                    var expertDetailsDto = new ExpertListDTO
                    {
                        Fullname = expertProfile.Fullname,
                        Specialization = expertProfile.Specialization,
                        AverageRating = averageRating,
                        TotalAppointments = completedAppointments.Count,
                        ProfileImageUrl = expertProfile.ProfileImageUrl,
                        TotalRatings = totalRatings
                    };
                    expertDetailsDtos.Add(expertDetailsDto);
                }

                response.Success = true;
                response.Data = expertDetailsDtos;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách thông tin chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách thông tin chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
