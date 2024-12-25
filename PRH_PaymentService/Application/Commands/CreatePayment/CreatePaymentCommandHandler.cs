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
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return BaseResponse<string>.BadRequest("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    return BaseResponse<string>.Unauthorized();
                }


                var paymentRequest = new PaymentRequest
                {
                    AppointmentId = request.PaymentPayloadDto.AppointmentId,
                    OrderCode = request.PaymentPayloadDto.OrderCode ,
                    Amount = request.PaymentPayloadDto.Amount,
                    Description = "request.PaymentPayloadDto.Description",
                    ReturnUrl = request.PaymentPayloadDto.ReturnUrl,
                    CancelUrl = request.PaymentPayloadDto.CancelUrl
                };

                var payOSResponse = await payOSService.CreatePaymentLink(paymentRequest);

                if (payOSResponse == null)
                {
                    return BaseResponse<string>.InternalServerError("Không thể tạo liên kết thanh toán.");
                }

                var payment = new Payment
                {
                    PaymentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    AppointmentId = request.PaymentPayloadDto.AppointmentId,
                    OrderCode = request.PaymentPayloadDto.OrderCode,
                    Amount = request.PaymentPayloadDto.Amount,
                    Status = (int)PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };
                await paymentRepository.Create(payment);

                return BaseResponse<string>.SuccessReturn(payOSResponse.PaymentUrl, "Đã tạo yêu cầu thanh toán thành công.");
            }
            catch 
            {
                return BaseResponse<string>.InternalServerError("Lỗi hệ thống: không thể tạo yêu cầu thanh toán.");
            }
        }
    }
}
