using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.RateExpert
{
    public class RateExpertCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<RateExpertCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(RateExpertCommand request, CancellationToken cancellationToken)
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
                    response.Message = "Không xác định được người dùng hiện tại.";
                    response.StatusCode = 401;
                    return response;
                }

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy lịch hẹn.";
                    response.StatusCode = 404;
                    return response;
                }

                if (appointment.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Người dùng không có quyền đánh giá lịch hẹn này.";
                    response.StatusCode = 403;
                    return response;
                }

                if (appointment.Status != 3)
                {
                    response.Success = false;
                    response.Message = "Chỉ có thể đánh giá sau khi tư vấn hoàn thành.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    response.Success = false;
                    response.Message = "Đánh giá phải nằm trong khoảng từ 1 đến 5.";
                    response.StatusCode = 400;
                    return response;
                }

                appointment.Rating = request.Rating;
                appointment.Comment = request.Comment;
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Đánh giá thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi đánh giá.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
