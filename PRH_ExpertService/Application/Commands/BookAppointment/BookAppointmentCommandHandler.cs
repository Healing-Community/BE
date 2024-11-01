using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.BookAppointment
{
    public class BookAppointmentCommandHandler(
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<BookAppointmentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
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
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null || availability.Status != 0)
                {
                    response.Success = false;
                    response.Message = "Lịch trống không tồn tại hoặc không khả dụng.";
                    response.StatusCode = 404;
                    return response;
                }

                // Cập nhật trạng thái của lịch trống sang "Chờ thanh toán"
                availability.Status = 1; // Đang chờ thanh toán
                availability.UpdatedAt = DateTime.UtcNow;
                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                // Trả về thông báo tạm thời
                response.Success = true;
                response.Data = "Yêu cầu đặt lịch của bạn đã được ghi nhận. Vui lòng chờ thanh toán.";
                response.StatusCode = 200;
                response.Message = "Đặt lịch thành công, chờ xử lý thanh toán.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi đặt lịch hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
