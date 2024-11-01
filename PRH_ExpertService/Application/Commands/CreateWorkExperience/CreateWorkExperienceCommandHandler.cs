using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Commands.CreateWorkExperience
{
    public class CreateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<CreateWorkExperienceCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.PositionTitle))
                {
                    response.Success = false;
                    response.Message = "Tên công ty và chức danh không được để trống.";
                    response.StatusCode = 400;
                    return response;
                }

                var workExperience = new WorkExperience
                {
                    WorkExperienceId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = request.ExpertProfileId,
                    CompanyName = request.CompanyName,
                    PositionTitle = request.PositionTitle,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await workExperienceRepository.Create(workExperience);

                response.Success = true;
                response.Data = workExperience.WorkExperienceId;
                response.Message = "Tạo kinh nghiệm làm việc thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra trong quá trình tạo kinh nghiệm làm việc.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}