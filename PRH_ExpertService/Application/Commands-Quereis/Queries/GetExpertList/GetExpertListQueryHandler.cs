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
                    // Kiểm tra trạng thái hồ sơ chuyên gia
                    if (expertProfile.Status != 1) // Approved
                    {
                        continue; // Bỏ qua các hồ sơ chưa được duyệt
                    }

                    // Lấy tất cả các cuộc hẹn của chuyên gia
                    var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfile.ExpertProfileId);
                    var completedAppointments = appointments.Where(a => a.Status == 3).ToList();

                    // Lọc các cuộc hẹn đã được đánh giá
                    var ratedAppointments = completedAppointments.Where(a => a.Rating.HasValue && a.Rating.Value > 0).ToList();

                    // Tính tổng số đánh giá
                    var totalRatings = ratedAppointments.Count;

                    // Tính trung bình đánh giá từ cơ sở dữ liệu
                    decimal averageRating = expertProfile.AverageRating;

                    var expertDetailsDto = new ExpertListDTO
                    {
                        ExpertId = expertProfile.ExpertProfileId,
                        Fullname = expertProfile.Fullname,
                        Specialization = expertProfile.Specialization,
                        AverageRating = averageRating,
                        TotalAppointments = completedAppointments.Count, // Tính từ danh sách cuộc hẹn
                        ProfileImageUrl = expertProfile.ProfileImageUrl,
                        TotalRatings = totalRatings // Tính từ danh sách cuộc hẹn đã được đánh giá
                    };
                    expertDetailsDtos.Add(expertDetailsDto);
                }

                // Sắp xếp danh sách chuyên gia theo tiêu chí
                var sortedExpertDetailsDtos = expertDetailsDtos
                    .OrderByDescending(e => e.AverageRating)
                    .ThenByDescending(e => e.TotalRatings)
                    .ThenByDescending(e => e.TotalAppointments)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                response.Success = true;
                response.Data = sortedExpertDetailsDtos;
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
