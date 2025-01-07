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
                return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn([],"Không tìm thấy thông tin thanh toán.");
            }

            var userIds = payments.Select(p => p.UserId).Distinct().ToList();

            var userDataReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRepeatedRequest, GetPaymentInfoListResponse>(
                "UserServiceUrl",
                async client => await client.GetUserPaymentInfoRepeatedAsync(new GetUserPaymentInfoRepeatedRequest { UserIds = { userIds } })
            );
            var userInfoResult = userDataReply?.PaymentInfos.ToDictionary(u => u.UserId);

            if (userInfoResult == null || !userInfoResult.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerDto>>.NotFound("Không tìm thấy thông tin người dùng. Có thể do người dùng này đã bị xóa.");
            }

            var appointmentIds = payments.Select(a => a.AppointmentId).Distinct().ToList();

            var appointmentDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsRequestRepeated, GetAppointmentsListResponse>(
                "ExpertServiceUrl",
                async client => await client.GetAllAppointmentsAsync(new GetAppointmentsRequestRepeated { AppointmentIds = { appointmentIds } })
            );
            var appointments = appointmentDataReply?.Appointments.ToList();

            if (appointments == null || !appointments.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn([],"Không tìm thấy thông tin lịch hẹn. Payment-Service");
            }

            var paymentManagerDtos = new List<PaymentManagerDto>(); 
            // Get Appointment from AppointmentService using grpc
            foreach (var payment in payments)
            {
                // Get appointment data

                var appointmentData = appointmentDataReply.Appointments.FirstOrDefault(a => a.AppointmentId == payment.AppointmentId);
                if (appointmentData == null)
                {
                    return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn([],"Không tìm thấy thông tin lịch hẹn. Payment-Service");
                }
                var userData = userInfoResult[payment.UserId];
                if (userData == null)
                {
                    return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn([],"Không tìm thấy thông tin người dùng.");
                }
                // Mapping
                var paymentManagerDto = new PaymentManagerDto
                {
                    // User
                    UserId = payment.UserId,
                    UserEmail = userData.UserEmail,
                    UserName = userData.UserName,

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
                    ExpertName = appointmentData.ExpertName,
                    ExpertEmail = appointmentData.ExpertEmail,
                    AppointmentDate = appointmentData.AppointmentDate,
                    StartTime = appointmentData.StartTime,
                    EndTime = appointmentData.EndTime,

                    // Modified
                    PaymentDate = payment.PaymentDate,
                    UpdatedAt = payment.UpdatedAt
                };
                paymentManagerDtos.Add(paymentManagerDto);
            }
            return BaseResponse<IEnumerable<PaymentManagerDto>>.SuccessReturn(paymentManagerDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
