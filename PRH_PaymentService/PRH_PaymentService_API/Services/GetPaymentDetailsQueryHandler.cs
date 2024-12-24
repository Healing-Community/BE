using Application.Commons;
using Application.Interfaces.Repositories;
using MediatR;
using NUlid;

namespace PRH_PaymentService_API.Services
{
    public record GetPaymentDetailsQuery(string PaymentId) : IRequest<BaseResponse<PaymentDetailsDTO>>;

    public class GetPaymentDetailsQueryHandler(
        IPaymentRepository paymentRepository,
        ExpertServiceGrpcClient expertServiceGrpcClient) : IRequestHandler<GetPaymentDetailsQuery, BaseResponse<PaymentDetailsDTO>>
    {
        public async Task<BaseResponse<PaymentDetailsDTO>> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<PaymentDetailsDTO>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // 1. Lấy Payment từ DB Payment Service
                var payment = await paymentRepository.GetByIdAsync(request.PaymentId);
                if (payment == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy Payment.";
                    response.StatusCode = 404;
                    return response;
                }

                // 2. Gọi gRPC sang Expert Service -> Lấy Appointment details
                var expertResp = await expertServiceGrpcClient
                    .GetAppointmentDetailsAsync(payment.AppointmentId);

                if (!expertResp.Success)
                {
                    // Xử lý nếu ExpertService trả về false
                    response.Success = false;
                    response.Message = $"Lấy chi tiết Appointment thất bại: {expertResp.Message}";
                    response.StatusCode = 400;
                    return response;
                }

                // 3. Gộp dữ liệu vào PaymentDetailsDTO
                var paymentDetails = new PaymentDetailsDTO
                {
                    PaymentId = payment.PaymentId,
                    AppointmentId = payment.AppointmentId,
                    Amount = payment.Amount,

                    ExpertName = expertResp.ExpertName,
                    ExpertEmail = expertResp.ExpertEmail,
                    AppointmentDate = expertResp.AppointmentDate, // đang là string
                    StartTime = expertResp.StartTime,
                    EndTime = expertResp.EndTime
                };

                // 4. Trả kết quả
                response.Success = true;
                response.Data = paymentDetails;
                response.Message = "Lấy Payment + Appointment thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Đã xảy ra lỗi: {ex.Message}";
                response.StatusCode = 500;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
