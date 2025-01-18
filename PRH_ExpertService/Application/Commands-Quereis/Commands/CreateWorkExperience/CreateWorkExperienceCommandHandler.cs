using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.CreateWorkExperience
{
    public class CreateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IHttpContextAccessor httpContextAccessor,
        IExpertProfileRepository expertProfileRepository)
        : IRequestHandler<CreateWorkExperienceCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                // Kiểm tra trạng thái hồ sơ chuyên gia
                var expertProfile = await expertProfileRepository.GetByIdAsync(userId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Errors.Add("Hồ sơ chuyên gia không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                //if (expertProfile.Status != 1) // Approved
                //{
                //    response.Success = false;
                //    response.Errors.Add("Hồ sơ của bạn chưa được duyệt. Vui lòng hoàn tất thông tin cá nhân và tải lên chứng chỉ, sau đó chờ phê duyệt.");
                //    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                //    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                //    return response;
                //}

                // Validate input
                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    response.Errors.Add("Tên công ty không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(request.PositionTitle))
                {
                    response.Errors.Add("Chức danh không được để trống.");
                }

                if (request.StartDate >= request.EndDate)
                {
                    response.Errors.Add("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
                }

                // Chuyển đổi StartDate và EndDate từ DateOnly sang DateTime
                var startDateTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
                var endDateTime = request.EndDate.ToDateTime(TimeOnly.MinValue);

                // Kiểm tra nếu EndDate vượt quá thời gian hiện tại
                if (endDateTime > DateTime.UtcNow.AddHours(7))
                {
                    response.Errors.Add("Ngày kết thúc không thể lớn hơn thời gian hiện tại.");
                }

                if (response.Errors.Any())
                {
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
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
                    response.Success = false;
                    response.Errors.Add("Kinh nghiệm làm việc này đã tồn tại.");
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    return response;
                }

                bool hasTimeConflict = existingExperiences.Any(e =>
                    e.StartDate < request.EndDate && e.EndDate > request.StartDate);

                if (hasTimeConflict)
                {
                    response.Success = false;
                    response.Errors.Add("Khoảng thời gian này xung đột với một kinh nghiệm làm việc khác.");
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
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
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
