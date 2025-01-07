using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.CreateExpertProfile
{
    public class CreateExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CreateExpertProfileCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateExpertProfileCommand request, CancellationToken cancellationToken)
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

                // Lấy UserId từ HTTP context
                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                // Lấy email từ HTTP context
                var email = Authentication.GetUserEmailFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Errors.Add("Không xác định được email của người dùng hiện tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Kiểm tra xem hồ sơ chuyên gia đã tồn tại chưa
                var existingProfile = await expertProfileRepository.GetByIdAsync(userId);
                if (existingProfile != null)
                {
                    response.Success = false;
                    response.Errors.Add("Hồ sơ chuyên gia đã tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Tạo hồ sơ chuyên gia mới
                var newExpertProfile = new ExpertProfile
                {
                    ExpertProfileId = userId,
                    UserId = userId,
                    Email = email,
                    Specialization = request.Specialization,
                    ExpertiseAreas = request.ExpertiseAreas,
                    Bio = request.Bio,
                    Status = 0, // PendingApproval
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await expertProfileRepository.Create(newExpertProfile);

                response.Success = true;
                response.Message = "Hồ sơ chuyên gia đã được tạo thành công.";
                response.Data = newExpertProfile.ExpertProfileId;
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
