using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;
using NUlid;

namespace Application.Commands.UpdateWorkExperience
{
    public class UpdateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<UpdateWorkExperienceCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var workExperience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);
                if (workExperience == null)
                {
                    response.Success = false;
                    response.Message = "Kinh nghiệm làm việc không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                workExperience.CompanyName = request.CompanyName;
                workExperience.PositionTitle = request.PositionTitle;
                workExperience.StartDate = request.StartDate;
                workExperience.EndDate = request.EndDate;
                workExperience.Description = request.Description;
                workExperience.UpdatedAt = DateTime.UtcNow;

                await workExperienceRepository.Update(request.WorkExperienceId, workExperience);

                response.Success = true;
                response.Data = true;
                response.Message = "Cập nhật kinh nghiệm làm việc thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật kinh nghiệm làm việc.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}