using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetAppointmentRatingStatus
{
    public class GetAppointmentRatingStatusQueryHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetAppointmentRatingStatusQuery, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(GetAppointmentRatingStatusQuery request, CancellationToken cancellationToken)
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
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = 401;
                    return response;
                }

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null || appointment.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Lịch hẹn không tồn tại hoặc không thuộc về người dùng hiện tại.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = appointment.Rating.HasValue && appointment.Rating.Value > 0;
                response.StatusCode = 200;
                response.Message = "Kiểm tra trạng thái đánh giá của lịch hẹn thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi kiểm tra trạng thái đánh giá của lịch hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
