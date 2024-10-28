using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.BookAppointment
{
    public class BookAppointmentCommandHandler(
            IExpertAvailabilityRepository expertAvailabilityRepository,
            IExpertProfileRepository expertProfileRepository,
            IPaymentService paymentService,
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

                if (string.IsNullOrEmpty(request.ExpertAvailabilityId))
                {
                    response.Success = false;
                    response.Message = "ID của lịch trống không hợp lệ.";
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

                if (availability.AvailableDate < DateTime.UtcNow.Date ||
                    (availability.AvailableDate == DateTime.UtcNow.Date && availability.EndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Success = false;
                    response.Message = "Thời gian của lịch trống đã qua hoặc không hợp lệ.";
                    response.StatusCode = 400;
                    return response;
                }

                if (availability.StartTime >= availability.EndTime)
                {
                    response.Success = false;
                    response.Message = "Thời gian bắt đầu phải trước thời gian kết thúc.";
                    response.StatusCode = 400;
                    return response;
                }

                var expertProfile = await expertProfileRepository.GetByIdAsync(availability.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy thông tin chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                if (expertProfile.Status != 1)
                {
                    response.Success = false;
                    response.Message = "Chuyên gia không khả dụng để đặt lịch.";
                    response.StatusCode = 400;
                    return response;
                }

                // Cập nhật trạng thái của lịch trống sang "Đang chờ thanh toán"
                availability.Status = 1; // Đang chờ thanh toán
                availability.UpdatedAt = DateTime.UtcNow;
                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                // Giả định rằng mỗi lần tư vấn có giá cố định (Ví dụ: 500,000 VND)
                decimal consultationFee = 500000m;

                // Bước 1: Tạo yêu cầu thanh toán giả lập
                string paymentSessionId = Ulid.NewUlid().ToString();
                var qrCodeLink = $"https://example.com/qrcode/{paymentSessionId}";

                var paymentResult = await paymentService.CreatePaymentRequestAsync(paymentSessionId, consultationFee, userId, qrCodeLink);

                if (!paymentResult.Success)
                {
                    // Nếu tạo yêu cầu thanh toán thất bại, hủy lịch trống
                    availability.Status = 0; // Đặt lại trạng thái ban đầu
                    await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                    response.Success = false;
                    response.Message = "Không thể tạo yêu cầu thanh toán.";
                    response.StatusCode = 500;
                    return response;
                }

                // Bước 2: Trả về thông tin QR Code để thanh toán
                response.Success = true;
                response.Data = qrCodeLink; // Trả về đường dẫn QR Code để người dùng thanh toán
                response.StatusCode = 200;
                response.Message = "Yêu cầu thanh toán đã được tạo thành công. Vui lòng quét QR để hoàn tất thanh toán.";
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
