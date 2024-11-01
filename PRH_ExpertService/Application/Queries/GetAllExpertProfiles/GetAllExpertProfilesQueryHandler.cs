using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllExpertProfiles
{
    public class GetAllExpertProfilesQueryHandler(IExpertProfileRepository expertProfileRepository)
        : IRequestHandler<GetAllExpertProfilesQuery, BaseResponse<IEnumerable<ExpertProfile>>>
    {
        public async Task<BaseResponse<IEnumerable<ExpertProfile>>> Handle(GetAllExpertProfilesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ExpertProfile>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var expertProfiles = await expertProfileRepository.GetsAsync();

                response.Success = true;
                response.Data = expertProfiles;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách hồ sơ chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
