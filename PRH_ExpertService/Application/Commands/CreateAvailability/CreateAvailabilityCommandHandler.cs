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

                if (request.EndTime <= request.StartTime)
                {
                    response.Success = false;
                    response.Message = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.AvailableDate < DateTime.UtcNow.Date ||
                    (request.AvailableDate == DateTime.UtcNow.Date && request.EndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Success = false;
                    response.Message = "Ngày và thời gian của lịch trống phải là trong tương lai.";
                    response.StatusCode = 400;
                    return response;
                }

                var overlappingAvailability = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    request.ExpertProfileId, request.AvailableDate, request.StartTime, request.EndTime);

                if (overlappingAvailability != null)
                {
                    response.Success = false;
                    response.Message = "Khoảng thời gian này đã trùng với lịch trống hiện tại. Vui lòng chọn thời gian khác.";
                    response.StatusCode = 409;
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
