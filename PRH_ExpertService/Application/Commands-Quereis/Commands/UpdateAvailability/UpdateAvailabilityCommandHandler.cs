using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.UpdateAvailability
{
    public class UpdateAvailabilityCommandHandler(
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateAvailabilityCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
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

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lịch trống không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Success = false;
                    response.Errors.Add("Thời gian kết thúc phải sau thời gian bắt đầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewAvailableDate < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) ||
                   (request.NewAvailableDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) &&
                    request.NewEndTime <= TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7))))
                {
                    response.Success = false;
                    response.Errors.Add("Ngày và thời gian của lịch trống phải là trong tương lai.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var overlapping = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    availability.ExpertProfileId, request.NewAvailableDate, request.NewStartTime, request.NewEndTime);

                if (overlapping != null && overlapping.ExpertAvailabilityId != availability.ExpertAvailabilityId)
                {
                    response.Success = false;
                    response.Errors.Add("Thời gian trống bị trùng lặp với một lịch trống khác.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật lịch trống
                availability.AvailableDate = request.NewAvailableDate;
                availability.StartTime = request.NewStartTime;
                availability.EndTime = request.NewEndTime;
                availability.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Cập nhật lịch trống thành công.";
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
