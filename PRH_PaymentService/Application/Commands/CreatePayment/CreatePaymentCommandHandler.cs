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
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                long timestampPart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                int randomPart = new Random().Next(1000, 9999);

                string orderCodeString = $"{timestampPart}{randomPart}";
                if (orderCodeString.Length > 15)
                {
                    orderCodeString = orderCodeString.Substring(0, 15);
                }

                long orderCode = long.Parse(orderCodeString);

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

                if (payOSResponse == null)
                {
                    response.Success = false;
                    response.Message = "Không thể tạo liên kết thanh toán.";
                    response.StatusCode = 500;
                    return response;
                }

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
