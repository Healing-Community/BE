using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.UpdateWorkExperience
{
    public class UpdateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<UpdateWorkExperienceCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
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

                var workExperience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);
                if (workExperience == null)
                {
                    response.Success = false;
                    response.Errors.Add("Kinh nghiệm làm việc không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (workExperience.ExpertProfileId != userId)
                {
                    response.Success = false;
                    response.Errors.Add("Người dùng không có quyền cập nhật kinh nghiệm làm việc này.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    return response;
                }

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

                if (response.Errors.Any())
                {
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var existingExperiences = await workExperienceRepository.GetWorkExperiencesByExpertIdAsync(userId);

                // Loại bỏ bản ghi hiện tại khỏi danh sách kiểm tra
                existingExperiences = existingExperiences.Where(e => e.WorkExperienceId != request.WorkExperienceId);

                if (existingExperiences.Any(e =>
                    e.CompanyName == request.CompanyName &&
                    e.PositionTitle == request.PositionTitle &&
                    e.StartDate == request.StartDate &&
                    e.EndDate == request.EndDate))
                {
                    response.Success = false;
                    response.Errors.Add("Kinh nghiệm làm việc này đã tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (existingExperiences.Any(e =>
                    e.StartDate < request.EndDate && e.EndDate > request.StartDate))
                {
                    response.Success = false;
                    response.Errors.Add("Khoảng thời gian này xung đột với một kinh nghiệm làm việc khác.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật kinh nghiệm làm việc
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
