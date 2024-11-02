using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;
using NUlid;

namespace Application.Commands.UpdateWorkExperience
{
    public class UpdateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<UpdateWorkExperienceCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var workExperience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);
                if (workExperience == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Kinh nghiệm làm việc không tồn tại.",
                        Field = "WorkExperienceId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                workExperience.CompanyName = request.CompanyName;
                workExperience.PositionTitle = request.PositionTitle;
                workExperience.StartDate = request.StartDate;
                workExperience.EndDate = request.EndDate;
                workExperience.Description = request.Description;
                workExperience.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await workExperienceRepository.Update(request.WorkExperienceId, workExperience);

                response.Success = true;
                response.Data = true;
                response.Message = "Cập nhật kinh nghiệm làm việc thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật kinh nghiệm làm việc.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
