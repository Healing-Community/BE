using Application.Commons;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public class UpdatePaymentStatusCommandHandler(IGrpcHelper grpcHelper,IPaymentRepository paymentRepository) : IRequestHandler<UpdatePaymentStatusCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
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
                    var UpdateAppointmentReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        async client => await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 })
                    );
                    var UpdateExpertAvailabilityReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        async client => await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 0 })
                    );
                    if (UpdateAppointmentReply == null || UpdateExpertAvailabilityReply == null)
                    {
                        return BaseResponse<string>.NotFound("Lịch hẹn không tồn tại.");
                    }
                    //End Grpc
                    #endregion
                    return BaseResponse<string>.SuccessReturn("Hủy thanh toán thành công.");
                }
                else if (request.Status == (int)Domain.Enum.PaymentStatus.Paid)
                {
                    await paymentRepository.UpdateStatus(request.OrderCode, Domain.Enum.PaymentStatus.Paid);
                    #region ExpertPaymentService gRPC
                    // Gửi appointmentId để cancel appointment
                    var UpdateAppointmentReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        async client => await client.UpdateAppointmentAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 1 })
                    );
                    var UpdateExpertAvailabilityReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, UpdateResponse>(
                        async client => await client.UpdateExpertAvailabilityAsync(new GetAppointmentsRequest { AppointmentId = request.AppointmentId, Status = 2 })
                    );
                    if (UpdateAppointmentReply == null || UpdateExpertAvailabilityReply == null)
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
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
    }
}
