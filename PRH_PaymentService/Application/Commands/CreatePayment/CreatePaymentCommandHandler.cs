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
using Grpc.Net.Client;
using ExpertPaymentService;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IConfiguration configuration,
        PayOS payOSService,
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
                #region ExpertPaymentService gRPC
                // Grpc qua expert để lấy thông tin lịch hẹn đồng thời kiểm tra xem lịch hẹn có tồn tại không
                var expertServiceUrl = configuration["ExpertServiceUrl"];
                if(expertServiceUrl == null)
                {
                    return BaseResponse<string>.InternalServerError("Đã xảy ra lỗi khi tạo yêu cầu thanh toán.");
                }
                var httpHandler = new HttpClientHandler
                {
                    // For local development only - allows insecure HTTP/2
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using var channel = GrpcChannel.ForAddress(expertServiceUrl, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });

                var client = new ExpertService.ExpertServiceClient(channel);
                var reply = await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = request.PaymentRequest.AppointmentId });
                if (reply == null)
                {
                    return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                }
                //End Grpc
                #endregion
                var amount = reply.Amount;
                var orderCode = GenerateOrderCode();

                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext == null)
                {
                    return BaseResponse<string>.InternalServerError("Đã xảy ra lỗi khi tạo yêu cầu thanh toán.");
                }
                var cancelApiUrl = GetApiUrl(httpContext, "payment", "redirect", orderCode.ToString(), request.PaymentRequest.RedirectUrl ?? "http://example.com/error", true, request.PaymentRequest.AppointmentId);
                var returnApiUrl = GetApiUrl(httpContext, "payment", "redirect", orderCode.ToString(), request.PaymentRequest.RedirectUrl ?? "http://example.com/error", false, request.PaymentRequest.AppointmentId);

                // convert to UTC+7
                long expiredAt = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
                var paymentData = new PaymentData(orderCode, amount, "Thanh toán lịch hẹn", [], cancelApiUrl, returnApiUrl,expiredAt:expiredAt );

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
        public string GetApiUrl(HttpContext httpContext, string controllerName = "MyController", string actionName = "GetData", string param = "1", string redirectUrl = "http://localhost:3000", bool isCancel = false, string appointmentId = "1")
        {
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host;
            return $"{scheme}://{host}/api/{controllerName}/{actionName}/{param}/{redirectUrl}/{isCancel}/{appointmentId}";
        }
    }
}
