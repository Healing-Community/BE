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

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.expertAvailabilityId);
                if (availability == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lịch trống không tồn tại.",
                        Field = "AvailabilityId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
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
                    response.StatusCode = 400;
                    return response;
                }

                if (request.NewAvailableDate < DateTime.UtcNow.Date ||
                   (request.NewAvailableDate == DateTime.UtcNow.Date && request.NewEndTime <= DateTime.UtcNow.TimeOfDay))
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
                    response.StatusCode = 400;
                    return response;
                }

                availability.AvailableDate = request.NewAvailableDate;
                availability.StartTime = request.NewStartTime;
                availability.EndTime = request.NewEndTime;
                availability.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
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
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
