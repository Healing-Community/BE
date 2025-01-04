using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.CreateWorkExperience
{
    public class CreateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IHttpContextAccessor httpContextAccessor,
        IExpertProfileRepository expertProfileRepository)
        : IRequestHandler<CreateWorkExperienceCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                // Kiểm tra trạng thái hồ sơ chuyên gia
                var expertProfile = await expertProfileRepository.GetByIdAsync(userId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Hồ sơ chuyên gia không tồn tại.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (expertProfile.Status != 1) // Approved
                {
                    response.Success = false;
                    response.Message = "Hồ sơ của bạn chưa được duyệt. Vui lòng hoàn tất thông tin cá nhân và tải lên chứng chỉ, sau đó chờ phê duyệt.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Tên công ty không được để trống.",
                        Field = "CompanyName"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.PositionTitle))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Chức danh không được để trống.",
                        Field = "PositionTitle"
                    });
                }

                if (request.StartDate >= request.EndDate)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc.",
                        Field = "StartDate"
                    });
                }

                if (response.Errors.Any())
                {
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = "Dữ liệu đầu vào không hợp lệ.";
                    return response;
                }

                var existingExperiences = await workExperienceRepository.GetWorkExperiencesByExpertIdAsync(userId);

                bool isDuplicate = existingExperiences.Any(e =>
                    e.CompanyName == request.CompanyName &&
                    e.PositionTitle == request.PositionTitle &&
                    e.StartDate == request.StartDate &&
                    e.EndDate == request.EndDate);

                if (isDuplicate)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Kinh nghiệm làm việc này đã tồn tại.",
                        Field = "WorkExperience"
                    });
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = "Kinh nghiệm làm việc trùng lặp.";
                    return response;
                }

                bool hasTimeConflict = existingExperiences.Any(e =>
                    e.StartDate < request.EndDate && e.EndDate > request.StartDate);

                if (hasTimeConflict)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Khoảng thời gian này xung đột với một kinh nghiệm làm việc khác.",
                        Field = "TimeConflict"
                    });
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = "Xung đột thời gian với kinh nghiệm làm việc khác.";
                    return response;
                }

                var workExperience = new WorkExperience
                {
                    WorkExperienceId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = userId,
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
                response.StatusCode = StatusCodes.Status200OK;
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
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
