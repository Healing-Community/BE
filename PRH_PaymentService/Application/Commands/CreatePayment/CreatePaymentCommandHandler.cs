using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler(
        IPayOSService payOSService,
        IPaymentRepository paymentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreatePaymentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
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
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                long timestampPart = long.Parse(DateTimeOffset.Now.ToString("ffffff")); // 6 chữ số microseconds
                int userIdPart = Math.Abs(userId.GetHashCode() % 1000); // Lấy 3 chữ số cuối của hash
                int randomPart = new Random().Next(100, 999); // Thêm 3 chữ số ngẫu nhiên

                long orderCode = long.Parse($"{timestampPart}{userIdPart}{randomPart}");

                var paymentRequest = new PaymentRequest
                {
                    AppointmentId = request.AppointmentId,
                    OrderCode = orderCode,
                    Amount = request.Amount,
                    Description = request.Description,
                    ReturnUrl = request.ReturnUrl,
                    CancelUrl = request.CancelUrl
                };

                var payOSResponse = await payOSService.CreatePaymentLink(paymentRequest);

                var payment = new Payment
                {
                    PaymentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    AppointmentId = request.AppointmentId,
                    OrderCode = orderCode,
                    Amount = request.Amount,
                    Status = (int)PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };
                await paymentRepository.Create(payment);

                // Trả về URL thanh toán
                response.Success = true;
                response.Data = payOSResponse.PaymentUrl;
                response.StatusCode = 200;
                response.Message = "Đã tạo yêu cầu thanh toán thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi tạo yêu cầu thanh toán.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}