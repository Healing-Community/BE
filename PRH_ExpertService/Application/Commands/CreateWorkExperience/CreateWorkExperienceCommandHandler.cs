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
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CreateWorkExperienceCommand, DetailBaseResponse<string>>
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
                    response.StatusCode = 409; // Conflict
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
