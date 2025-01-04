using Application.Commons;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System.Security.Claims;
using Net.payOS;
using Net.payOS.Types;
using ExpertPaymentService;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IGrpcHelper grpcHelper,
        PayOS payOSService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreatePaymentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return BaseResponse<string>.Unauthorized();
                }
                // Grpc qua expert để lấy thông tin lịch hẹn đồng thời kiểm tra xem lịch hẹn có tồn tại không
                var reply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, GetAppointmentsResponse>(
                    "ExpertServiceUrl",
                    async client => await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = request.PaymentRequest.AppointmentId })
                );
                if (reply == null)
                {
                    return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                }
                // lấy thông tin lịch hẹn để tạo payment
                var amount = reply.Amount;
                var orderCode = GenerateOrderCode();

                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext == null)
                {
                    return BaseResponse<string>.InternalServerError("Đã xảy ra lỗi khi tạo yêu cầu thanh toán.");
                }
                var cancelApiUrl = GetApiUrl(httpContext, "payment", "redirect", orderCode.ToString(), true, request.PaymentRequest.AppointmentId, request.PaymentRequest.RedirectUrl ?? "http://example.com/error");
                var returnApiUrl = GetApiUrl(httpContext, "payment", "redirect", orderCode.ToString(), false, request.PaymentRequest.AppointmentId, request.PaymentRequest.RedirectUrl ?? "http://example.com/error");

                // convert to UTC+7
                long expiredAt = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                var paymentData = new PaymentData(orderCode, amount, "Thanh toán lịch hẹn", [], cancelApiUrl, returnApiUrl, expiredAt: expiredAt);

                var payOSResponse = await payOSService.createPaymentLink(paymentData);

                if (payOSResponse == null)
                {
                    return BaseResponse<string>.InternalServerError("Đã xảy ra lỗi khi tạo yêu cầu thanh toán.");
                }

                // Create payment 
                var payment = new Payment
                {
                    AppointmentId = request.PaymentRequest.AppointmentId,
                    PaymentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    OrderCode = orderCode,
                    Amount = amount,
                    Status = (int)PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow + TimeSpan.FromHours(7),
                    UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                };
                // Save payment to database
                await paymentRepository.Create(payment);

                return BaseResponse<string>.SuccessReturn(payOSResponse.checkoutUrl);
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
        public static long GenerateOrderCode()
        {
            long timestampPart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int randomPart = new Random().Next(1000, 9999);

            string orderCodeString = $"{timestampPart}{randomPart}";
            if (orderCodeString.Length > 15)
            {
                orderCodeString = orderCodeString.Substring(0, 15);
            }
            return long.Parse(orderCodeString);
        }
        public string GetApiUrl(HttpContext httpContext, string controllerName = "MyController", string actionName = "GetData", string orderCode = "1", bool isCancel = false, string appointmentId = "1", string redirectUrl = "http://localhost:3000")
        {
            // var scheme = httpContext.Request.Scheme;
            // var host = httpContext.Request.Host;

            //return $"{scheme}://{host}/api/{controllerName}/{actionName}/{orderCode}/{isCancel}/{appointmentId}?redirectUrl={redirectUrl}";
            var callbackUrl = configuration.GetSection("CallbackUrls");
            var host = callbackUrl["Host"];
            return $"{host}/payment/api/{controllerName}/{actionName}/{orderCode}/{isCancel}/{appointmentId}?redirectUrl={redirectUrl}";
        }
    }
}
