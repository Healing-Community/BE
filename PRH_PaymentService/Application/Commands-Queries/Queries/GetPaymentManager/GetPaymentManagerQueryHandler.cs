using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using UserPaymentService;

namespace Application.Commands_Queries.Queries.GetPaymentManager;

public class GetPaymentManagerQueryHandler(IGrpcHelper grpcHelper,IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentManagerQuery, BaseResponse<IEnumerable<PaymentManagerDto>>>
{
    public async Task<BaseResponse<IEnumerable<PaymentManagerDto>>> Handle(GetPaymentManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var payments = await paymentRepository.GetsAsync();
            if (payments == null)
            {
                return BaseResponse<IEnumerable<PaymentManagerDto>>.NotFound("Không tìm thấy thông tin thanh toán.");
            }
            // Get Appointment from AppointmentService using grpc
            foreach (var payment in payments)
            {
                var appointmentDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequest, GetAppointmentsResponse>(
                    "ExpertServiceUrl",
                    async client => await client.GetAppointmentsAsync(new GetAppointmentsRequest { AppointmentId = payment.AppointmentId })
                );

                var userDataReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRequest, GetPaymentInfoResponse>(
                    "UserServiceUrl",
                    async client => await client.GetUserPaymentInfoAsync(new GetUserPaymentInfoRequest { UserId = payment.UserId })
                );

                if (appointmentDataReply == null || userDataReply == null)
                {
                    return BaseResponse<IEnumerable<PaymentManagerDto>>.NotFound("Lịch hẹn không tồn tại.");
                }
                // Mapping
                var paymentManagerDto = new PaymentManagerDto
                {
                    // User
                    UserId = payment.UserId,
                    UserEmail = userDataReply.UserEmail,
                    UserName = userDataReply.UserName,

                    // Payment
                    PaymentId = payment.PaymentId,
                    AppointmentId = payment.AppointmentId,
                    Amount = payment.Amount,
                    OrderCode = payment.OrderCode,
                    Status = payment.Status,
                    ExpertPaymentQrCodeLink = payment.ExpertPaymentQrCodeLink,
                    UserPaymentQrCodeLink = payment.UserPaymentQrCodeLink,
                    PaymentDetail = payment.PaymentDetail,

                    // Appointment
                    ExpertName = appointmentDataReply.ExpertName,
                    ExpertEmail = appointmentDataReply.ExpertEmail,
                    AppointmentDate = appointmentDataReply.AppointmentDate,
                    StartTime = appointmentDataReply.StartTime,
                    EndTime = appointmentDataReply.EndTime,

                    // Modified
                    PaymentDate = payment.PaymentDate,
                    UpdatedAt = payment.UpdatedAt
                };
            }
            return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn();
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
