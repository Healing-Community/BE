using Application.Commons;
using Application.Interfaces.Services;
using Domain.Contracts;
using MediatR;
using NUlid;

namespace Application.Queries.GetPaymentStatus
{
    public class GetPaymentStatusQueryHandler(IPayOSService payOSService) : IRequestHandler<GetPaymentStatusQuery, BaseResponse<PaymentStatusResponse>>
    {
        public async Task<BaseResponse<PaymentStatusResponse>> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<PaymentStatusResponse>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var paymentStatus = await payOSService.GetPaymentStatus(request.OrderCode);
                response.Success = true;
                response.Data = paymentStatus;
                response.StatusCode = 200;
                response.Message = "Lấy trạng thái thanh toán thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lấy trạng thái thanh toán.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}