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

public class GetPaymentManagerUserQueryHandler(IHttpContextAccessor accessor, IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentManagerUserQuery, BaseResponse<IEnumerable<PaymentManagerUserDto>>>
{
    public async Task<BaseResponse<IEnumerable<PaymentManagerUserDto>>> Handle(GetPaymentManagerUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch appointments
            var appointmentsDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsByUserRequest, GetAppointmentsListResponse>(
                    "ExpertServiceUrl",
                    async client => await client.GetAppointmentsByUserAsync(new GetAppointmentsByUserRequest { UserId = userId })
                );

            var appointments = appointmentsDataReply?.Appointments.ToList();

            if (appointments == null || !appointments.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn([],"Không tìm thấy thông tin lịch hẹn. Payment-Service");
            }

            // Collect all appointmentIds for querying payments
            var appointmentIds = appointments.Select(a => a.AppointmentId).Distinct().ToList();

            var payments = await paymentRepository.GetsByPropertyAsync(x => appointmentIds.Contains(x.AppointmentId));
            if (payments == null || !payments.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn([],"Không tìm thấy thông tin thanh toán.");
            }

            var userIds = payments.Select(p => p.UserId).Distinct().ToList();

            var userDataReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRepeatedRequest, GetPaymentInfoListResponse>(
                "UserServiceUrl",
                async client => await client.GetUserPaymentInfoRepeatedAsync(new GetUserPaymentInfoRepeatedRequest { UserIds = { userIds } })
            );

            var userInfoResult = userDataReply?.PaymentInfos.ToDictionary(u => u.UserId);


            if (userInfoResult == null || !userInfoResult.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn([],"Không tìm thấy thông tin người dùng.");
            }


            var paymentManagerUserDtos = new List<PaymentManagerUserDto>();

            foreach(var appointment in appointments)
            {
                if (!userInfoResult.TryGetValue(appointment.UserId, out var userInfo))
                {
                    // Nếu không tìm thấy userInfo, bỏ qua appointment này và tiếp tục.
                    continue;
                }
                if(!payments.Any(p => p.AppointmentId == appointment.AppointmentId))
                {
                    // Nếu không tìm thấy payment, bỏ qua appointment này và tiếp tục.
                    continue;
                }
                var paymentManagerUserDto = new PaymentManagerUserDto
                {
                    // User
                    UserId = appointment.UserId,
                    UserEmail = userInfo.UserEmail,
                    UserName = userInfo.UserName,

                    // Payment
                    PaymentId = payments.First(p => p.AppointmentId == appointment.AppointmentId).PaymentId,
                    OrderCode = payments.First(p => p.AppointmentId == appointment.AppointmentId).OrderCode,
                    PaymentDate = payments.First(p => p.AppointmentId == appointment.AppointmentId).PaymentDate,
                    // Appointment
                    Amount = appointment.Amount,
                    AppointmentId = appointment.AppointmentId,
                    Status = appointment.Status, // Lấy status từ lịch hẹn (Appointment)
                    ExpertName = appointment.ExpertName,
                    ExpertEmail = appointment.ExpertEmail,
                    AppointmentDate = appointment.AppointmentDate,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,

                    // General
                    UpdatedAt = payments.First(p => p.AppointmentId == appointment.AppointmentId).UpdatedAt
                };
                paymentManagerUserDtos.Add(paymentManagerUserDto);
            }
            
            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn(paymentManagerUserDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.InternalServerError(e.Message);
        }
    }
}
