using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAvailability
{
    public class GetAvailabilityQueryHandler(IExpertAvailabilityRepository expertAvailabilityRepository) : IRequestHandler<GetAvailabilityQuery, BaseResponse<IEnumerable<ExpertAvailability>>>
    {
        public async Task<BaseResponse<IEnumerable<ExpertAvailability>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ExpertAvailability>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var availabilityList = await expertAvailabilityRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);

                response.Success = true;
                response.Data = availabilityList;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch trống thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
