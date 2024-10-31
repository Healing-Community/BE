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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateExpertProfileCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không xác định được người dùng hiện tại.";
                    response.StatusCode = 400;
                    return response;
                }

                var newExpertProfile = new ExpertProfile
                {
                    ExpertProfileId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    Specialization = request.Specialization,
                    ExpertiseAreas = request.ExpertiseAreas,
                    Bio = request.Bio,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await expertProfileRepository.Create(newExpertProfile);

                response.Success = true;
                response.Message = "Hồ sơ chuyên gia đã được tạo thành công.";
                response.Data = newExpertProfile.ExpertProfileId;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình tạo hồ sơ chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
