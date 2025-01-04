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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateAvailabilityCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lịch trống không tồn tại.",
                        Field = "AvailabilityId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu.",
                        Field = "EndTime"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewAvailableDate < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) ||
                   (request.NewAvailableDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) &&
                    request.NewEndTime <= TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7))))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Ngày và thời gian của lịch trống phải là trong tương lai.",
                        Field = "AvailableDate"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var overlapping = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    availability.ExpertProfileId, request.NewAvailableDate, request.NewStartTime, request.NewEndTime);

                if (overlapping != null && overlapping.ExpertAvailabilityId != availability.ExpertAvailabilityId)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Thời gian trống bị trùng lặp với một lịch trống khác.",
                        Field = "TimeRange"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
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
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật lịch trống.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
