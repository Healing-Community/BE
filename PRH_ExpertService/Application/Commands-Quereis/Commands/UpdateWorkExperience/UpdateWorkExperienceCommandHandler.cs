using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;
using NUlid;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UpdateWorkExperience
{
    public class UpdateWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<UpdateWorkExperienceCommand, DetailBaseResponse<bool>>
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
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.",
                        Field = "HttpContext"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không xác định được người dùng hiện tại.",
                        Field = "UserId"
                    });
                    response.Success = false;
                    response.StatusCode = 401;
                    return response;
                }

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

                if (workExperience.ExpertProfileId != userId)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Người dùng không có quyền cập nhật kinh nghiệm làm việc này.",
                        Field = "Authorization"
                    });
                    response.Success = false;
                    response.StatusCode = 403;
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

                if (response.Errors.Count > 0)
                {
                    response.Success = false;
                    response.StatusCode = 400;
                    response.Message = "Dữ liệu đầu vào không hợp lệ.";
                    return response;
                }

                var existingExperiences = await workExperienceRepository.GetWorkExperiencesByExpertIdAsync(userId);

                // Loại bỏ bản ghi hiện tại khỏi danh sách kiểm tra
                existingExperiences = existingExperiences.Where(e => e.WorkExperienceId != request.WorkExperienceId);

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
                    response.StatusCode = 409;
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
                    response.StatusCode = 409;
                    response.Message = "Xung đột thời gian với kinh nghiệm làm việc khác.";
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
