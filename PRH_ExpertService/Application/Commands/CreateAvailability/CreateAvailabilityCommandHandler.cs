using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.CreateAvailability
{
    public class CreateAvailabilityCommandHandler(
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateAvailabilityCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
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

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy hồ sơ của chuyên gia với ID: {request.ExpertProfileId}.";
                    response.StatusCode = 404;
                    return response;
                }

                var existingAvailability = await expertAvailabilityRepository.GetByDateAndTimeAsync(request.ExpertProfileId, request.AvailableDate, request.StartTime, request.EndTime);

                if (existingAvailability != null)
                {
                    response.Success = false;
                    response.Message = "Đã có lịch trống cho ngày và giờ này. Vui lòng chọn thời gian khác.";
                    response.StatusCode = 400;
                    return response;
                }

                var newAvailability = new ExpertAvailability
                {
                    ExpertAvailabilityId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = request.ExpertProfileId,
                    AvailableDate = request.AvailableDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await expertAvailabilityRepository.Create(newAvailability);

                response.Success = true;
                response.Message = "Lịch trống đã được tạo thành công.";
                response.Data = newAvailability.ExpertAvailabilityId;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
