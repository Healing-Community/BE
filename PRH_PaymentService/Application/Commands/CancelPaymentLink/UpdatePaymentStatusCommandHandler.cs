using Application.Commons;
using Application.Interfaces.Repositories;
using ExpertPaymentService;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.CancelPaymentLink
{
    public class UpdatePaymentStatusCommandHandler(IConfiguration configuration,IPaymentRepository paymentRepository) : IRequestHandler<UpdatePaymentStatusCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var expertServiceUrl = "http://host.docker.internal:5005/";
                var httpHandler = new HttpClientHandler
                {
                    // For local development only - allows insecure HTTP/2
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using var channel = GrpcChannel.ForAddress(expertServiceUrl, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });

                var client = new ExpertService.ExpertServiceClient(channel); // Tạo client để gọi đến service
                var payment = await paymentRepository.GetByOrderCodeAsync(request.OrderCode);
                if (payment == null)
                {
                    return BaseResponse<string>.NotFound("Không tìm thấy thông tin thanh toán.");
                }
                if (request.Status == (int)Domain.Enum.PaymentStatus.Cancelled)
                {
                    await paymentRepository.UpdateStatus(request.OrderCode, Domain.Enum.PaymentStatus.Cancelled);
                    // Setup grpc call to cancel appointment and expertAvailability in ExpertService
                    #region ExpertPaymentService gRPC
                    // Grpc qua expert để lấy thông tin lịch hẹn đồng thời kiểm tra xem lịch hẹn có tồn tại không
                    // Gửi appointmentId để cancel appointment
                    var reply = await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 });
                    var reply2 = await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 0 });
                    if (reply == null || reply2 == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    //End Grpc
                    #endregion
                    return BaseResponse<string>.SuccessReturn("Hủy thanh toán thành công." + "Hủy hẹn: " + reply.IsSucess + "Trả lịch về khả dụng: " + reply2.IsSucess);
                }
                else if (request.Status == (int)Domain.Enum.PaymentStatus.Paid)
                {
                    await paymentRepository.UpdateStatus(request.OrderCode, Domain.Enum.PaymentStatus.Paid);
                    #region ExpertPaymentService gRPC
                    // Gửi appointmentId để cancel appointment
                    var reply = await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 3 });
                    var reply2 = await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 });
                    if (reply == null || reply2 == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    //End Grpc
                    #endregion
                    return BaseResponse<string>.SuccessReturn("Thanh toán thành công.");
                }
                return BaseResponse<string>.BadRequest("Trạng thái thanh toán không hợp lệ.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.ToString());
            }
        }
    }
}
