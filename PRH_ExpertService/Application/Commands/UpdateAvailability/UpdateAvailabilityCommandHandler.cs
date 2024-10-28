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

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.AvailabilityId);
                if (availability == null)
                {
                    response.Success = false;
                    response.Message = "Lịch trống không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Success = false;
                    response.Message = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.NewAvailableDate < DateTime.UtcNow.Date ||
                   (request.NewAvailableDate == DateTime.UtcNow.Date && request.NewEndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Success = false;
                    response.Message = "Ngày và thời gian của lịch trống phải là trong tương lai.";
                    response.StatusCode = 400;
                    return response;
                }

                var overlapping = await expertAvailabilityRepository.GetOverlappingAvailabilityAsync(
                    availability.ExpertProfileId, request.NewAvailableDate, request.NewStartTime, request.NewEndTime);

                if (overlapping != null && overlapping.ExpertAvailabilityId != availability.ExpertAvailabilityId)
                {
                    response.Success = false;
                    response.Message = "Thời gian trống bị trùng lặp với một lịch trống khác.";
                    response.StatusCode = 400;
                    return response;
                }

                availability.AvailableDate = request.NewAvailableDate;
                availability.StartTime = request.NewStartTime;
                availability.EndTime = request.NewEndTime;
                availability.UpdatedAt = DateTime.UtcNow;

                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Cập nhật lịch trống thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
