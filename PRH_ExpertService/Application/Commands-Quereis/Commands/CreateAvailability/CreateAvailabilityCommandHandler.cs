using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.CreateAvailability
{
    public class CreateAvailabilityCommandHandler(
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor,
        IExpertProfileRepository expertProfileRepository)
        : IRequestHandler<CreateAvailabilityCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
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
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return response;
                }

                if (expertProfile.Status == 2) // Rejected
                {
                    response.Success = false;
                    response.Errors.Add("Hồ sơ của bạn đã bị từ chối. Bạn không thể tạo lịch trống.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (expertProfile.Status != 1) // Approved
                {
                    response.Success = false;
                    response.Errors.Add("Hồ sơ của bạn chưa được duyệt. Vui lòng hoàn tất thông tin cá nhân và tải lên chứng chỉ, sau đó chờ phê duyệt.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Kiểm tra thời gian kết thúc phải sau thời gian bắt đầu
                if (request.EndTime <= request.StartTime)
                {
                    response.Errors.Add("Thời gian kết thúc phải sau thời gian bắt đầu.");
                }

                // Kiểm tra thời gian đặt lịch phải trên hoặc bằng 30 phút
                if ((request.EndTime - request.StartTime).TotalMinutes < 30)
                {
                    response.Errors.Add("Thời gian đặt lịch phải trên hoặc bằng 30 phút.");
                }

                // Kiểm tra ngày và thời gian của lịch trống phải là trong tương lai
                if (request.AvailableDate < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) ||
                    (request.AvailableDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) &&
                     request.EndTime <= TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7))))
                {
                    response.Errors.Add("Ngày và thời gian của lịch trống phải là trong tương lai.");
                }

                // Kiểm tra giá tiền tối thiểu
                if (request.Amount < 10000)
                {
                    response.Errors.Add("Giá tiền tối thiểu là 10,000 VND.");
                }

                if (response.Errors.Any())
                {
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Kiểm tra trùng lặp lịch trống
                var overlappingAvailability = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    userId, request.AvailableDate, request.StartTime, request.EndTime);

                if (overlappingAvailability != null)
                {
                    response.Success = false;
                    response.Errors.Add("Khoảng thời gian này đã trùng với lịch trống hiện tại. Vui lòng chọn thời gian khác.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Tạo lịch trống mới
                var newAvailability = new ExpertAvailability
                {
                    ExpertAvailabilityId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = userId,
                    AvailableDate = request.AvailableDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = 0, // Available
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7),
                    Amount = request.Amount
                };

                await expertAvailabilityRepository.Create(newAvailability);

                response.Success = true;
                response.Message = "Lịch trống đã được tạo thành công.";
                response.Data = newAvailability.ExpertAvailabilityId;
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
