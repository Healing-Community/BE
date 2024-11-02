using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetWorkExperienceQuery
{
    public class GetWorkExperienceQueryHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<GetWorkExperienceQuery, BaseResponse<WorkExperience>>
    {
        public async Task<BaseResponse<WorkExperience>> Handle(GetWorkExperienceQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<WorkExperience>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var experience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);
                if (experience == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy kinh nghiệm làm việc.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = experience;
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