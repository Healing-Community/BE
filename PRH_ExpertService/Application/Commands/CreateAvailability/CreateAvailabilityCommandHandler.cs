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
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CreateAvailabilityCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
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
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = $"Không tìm thấy hồ sơ của chuyên gia với ID: {request.ExpertProfileId}.",
                        Field = "ExpertProfileId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                if (request.EndTime <= request.StartTime)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu.",
                        Field = "EndTime"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                if (request.AvailableDate < DateTime.UtcNow.Date ||
                    (request.AvailableDate == DateTime.UtcNow.Date && request.EndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Ngày và thời gian của lịch trống phải là trong tương lai.",
                        Field = "AvailableDate"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                var overlappingAvailability = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    request.ExpertProfileId, request.AvailableDate, request.StartTime, request.EndTime);

                if (overlappingAvailability != null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Khoảng thời gian này đã trùng với lịch trống hiện tại. Vui lòng chọn thời gian khác.",
                        Field = "TimeRange"
                    });
                    response.Success = false;
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await expertAvailabilityRepository.Create(newAvailability);

                response.Success = true;
                response.Message = "Lịch trống đã được tạo thành công.";
                response.Data = newAvailability.ExpertAvailabilityId;
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
                response.Message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
