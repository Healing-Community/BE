using Grpc.Core;
using ExpertService.gRPC;
using Application.Interfaces.Services;
using Application.Interfaces.Repository;

namespace PRH_ExpertService_API.Services
{
    public class ExpertServiceImpl(IEmailService emailService, IAppointmentRepository appointmentRepository) : ExpertService.gRPC.ExpertService.ExpertServiceBase
    {
        public override async Task<PaymentSuccessResponse> PaymentSuccess(PaymentSuccessRequest request, ServerCallContext context)
        {
            try
            {
                // Logic xử lý yêu cầu PaymentSuccess
                await Task.CompletedTask; // Placeholder for actual async logic

                return new PaymentSuccessResponse
                {
                    Success = true,
                    Message = "Đã xử lý thanh toán thành công và gửi email xác nhận."
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về phản hồi thất bại
                return new PaymentSuccessResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }
    }
}
