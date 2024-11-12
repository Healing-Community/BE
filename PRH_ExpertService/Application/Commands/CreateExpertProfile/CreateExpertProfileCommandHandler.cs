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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateExpertProfileCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(CreateExpertProfileCommand request, CancellationToken cancellationToken)
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await expertProfileRepository.Create(newExpertProfile);

                response.Success = true;
                response.Message = "Hồ sơ chuyên gia đã được tạo thành công.";
                response.Data = newExpertProfile.ExpertProfileId;
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
                response.Message = "Đã xảy ra lỗi trong quá trình tạo hồ sơ chuyên gia.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
