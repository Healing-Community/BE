using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetWorkExperienceQuery
{
    public class GetWorkExperienceQueryHandler(
        IWorkExperienceRepository workExperienceRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetWorkExperienceQuery, BaseResponse<IEnumerable<WorkExperience>>>
    {
        public async Task<BaseResponse<IEnumerable<WorkExperience>>> Handle(GetWorkExperienceQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<WorkExperience>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // Lấy ExpertProfileId từ token
                var expertProfileId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (string.IsNullOrEmpty(expertProfileId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác thực người dùng.";
                    response.StatusCode = 401;
                    return response;
                }

                // Lấy danh sách kinh nghiệm làm việc theo ExpertProfileId
                var experiences = await workExperienceRepository.GetByExpertProfileIdAsync(expertProfileId);

                response.Success = true;
                response.Data = experiences;
                response.StatusCode = 200;
                response.Message = "Lấy thông tin kinh nghiệm làm việc thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin kinh nghiệm làm việc.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
