using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllWorkExperiences
{
    public class GetAllWorkExperiencesQueryHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<GetAllWorkExperiencesQuery, BaseResponse<IEnumerable<WorkExperience>>>
    {
        public async Task<BaseResponse<IEnumerable<WorkExperience>>> Handle(GetAllWorkExperiencesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<WorkExperience>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var experiences = await workExperienceRepository.GetsAsync();

                response.Success = true;
                response.Data = experiences;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách kinh nghiệm làm việc thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách kinh nghiệm làm việc.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}