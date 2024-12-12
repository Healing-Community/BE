using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetWorkExperienceById
{
    public class GetWorkExperienceByIdQueryHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<GetWorkExperienceByIdQuery, BaseResponse<WorkExperience>>
    {
        public async Task<BaseResponse<WorkExperience>> Handle(GetWorkExperienceByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<WorkExperience>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var workExperience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);

                response.Success = true;
                response.Data = workExperience;
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
