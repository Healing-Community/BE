using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllExpertAvailabilities
{
    public class GetAllExpertAvailabilitiesQueryHandler(IExpertAvailabilityRepository expertAvailabilityRepository)
        : IRequestHandler<GetAllExpertAvailabilitiesQuery, BaseResponse<IEnumerable<ExpertAvailability>>>
    {
        public async Task<BaseResponse<IEnumerable<ExpertAvailability>>> Handle(GetAllExpertAvailabilitiesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ExpertAvailability>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var availabilities = await expertAvailabilityRepository.GetsAsync();

                response.Success = true;
                response.Data = availabilities;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch trống thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
