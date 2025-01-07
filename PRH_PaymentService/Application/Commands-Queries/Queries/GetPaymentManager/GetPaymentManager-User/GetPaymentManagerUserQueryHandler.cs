using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserPaymentService;

namespace Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_User;

public class GetPaymentManagerUserQueryHandler(IHttpContextAccessor accessor,IGrpcHelper grpcHelper,IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentManagerUserQuery, BaseResponse<IEnumerable<PaymentManagerUserDto>>>
{
    public async Task<BaseResponse<IEnumerable<PaymentManagerUserDto>>> Handle(GetPaymentManagerUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
             var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payments = await paymentRepository.GetsAsync();
            if (payments == null)
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.NotFound("Không tìm thấy thông tin thanh toán.");
            }
            var paymentManagerDtos = new List<PaymentManagerUserDto>(); 
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
                    return BaseResponse<IEnumerable<PaymentManagerUserDto>>.NotFound("Lịch hẹn không tồn tại.");
                }
                // Mapping
                var paymentManagerDto = new PaymentManagerUserDto
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
                paymentManagerDtos.Add(paymentManagerDto);
            }
            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn(paymentManagerDtos.Where(x => x.UserId == userId));
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
