using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;

namespace Application.Commands.CreateWorkExperience
{
    public class CreateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<CreateWorkExperienceCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không tìm thấy hồ sơ chuyên gia.",
                        Field = "ExpertProfileId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.PositionTitle))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Tên công ty không được để trống.",
                        Field = "CompanyName"
                    });
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Chức danh không được để trống.",
                        Field = "PositionTitle"
                    });
                    response.Success = false;
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await workExperienceRepository.Create(workExperience);

                response.Success = true;
                response.Data = workExperience.WorkExperienceId;
                response.Message = "Tạo kinh nghiệm làm việc thành công.";
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
                response.Message = "Có lỗi xảy ra trong quá trình tạo kinh nghiệm làm việc.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
