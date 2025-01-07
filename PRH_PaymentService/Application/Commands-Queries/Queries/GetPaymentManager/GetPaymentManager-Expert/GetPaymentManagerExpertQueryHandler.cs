
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ExpertPaymentService;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserPaymentService;

namespace Application.Commands_Queries.Queries.GetPaymentManager.GetPaymentManager_Expert;

public class GetPaymentManagerExpertQueryHandler(IHttpContextAccessor accessor, IGrpcHelper grpcHelper, IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentManagerExpertQuery, BaseResponse<IEnumerable<PaymentManagerUserDto>>>
{
    public async Task<BaseResponse<IEnumerable<PaymentManagerUserDto>>> Handle(GetPaymentManagerExpertQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch appointments
            var appointmentsDataReply = await grpcHelper.ExecuteGrpcCallAsync<ExpertService.ExpertServiceClient, GetAppointmentsByExpertRequest, GetAppointmentsListResponse>(
                    "ExpertServiceUrl",
                    async client => await client.GetAppointmentsByExpertAsync(new GetAppointmentsByExpertRequest { ExpertId = userId })
                );
            var appointments = appointmentsDataReply?.Appointments.ToList();
            if (appointments == null || !appointments.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.NotFound("Không tìm thấy thông tin lịch hẹn.");
            }

            var paymentManagerUserDtos = new List<PaymentManagerUserDto>();

            // Collect all appointmentIds for querying payments
            var appointmentIds = appointments.Select(a => a.AppointmentId).Distinct().ToList();

            // Fetch payments based on appointments
            var payments = await paymentRepository.GetsByPropertyAsync(x => appointmentIds.Contains(x.AppointmentId));
            if (payments == null || !payments.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.NotFound("Không tìm thấy thông tin thanh toán.");
            }
            
            // Collect all userIds from payments
            var userIds = payments.Select(p => p.UserId).Distinct().ToList();

            // Fetch user information once using gRPC
            var userDataReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRepeatedRequest, GetPaymentInfoListResponse>(
                "UserServiceUrl",
                async client => await client.GetUserPaymentInfoRepeatedAsync(new GetUserPaymentInfoRepeatedRequest { UserIds = { userIds } })
            );


            var userInfoResult = userDataReply?.PaymentInfos.ToDictionary(u => u.UserId);
            if (userInfoResult == null || !userInfoResult.Any())
            {
                return BaseResponse<IEnumerable<PaymentManagerUserDto>>.NotFound("Không tìm thấy thông tin người dùng.");
            }

            // Map to collection of PaymentManagerUserDto
            foreach (var appointment in appointments)
            {
                var relatedPayments = payments.Where(p => p.AppointmentId == appointment.AppointmentId).ToList();

                foreach (var pay in relatedPayments)
                {
                    if (userInfoResult.TryGetValue(pay.UserId, out var userInfo))
                    {
                        var paymentManagerUserDto = new PaymentManagerUserDto
                        {
                            // User
                            UserId = pay.UserId,
                            UserEmail = userInfo.UserEmail,
                            UserName = userInfo.UserName,

                            // Payment
                            PaymentId = pay.PaymentId,
                            AppointmentId = pay.AppointmentId,
                            Amount = pay.Amount,
                            OrderCode = pay.OrderCode,
                            Status = pay.Status,
                            PaymentDetail = pay.PaymentDetail,

                            // Appointment
                            ExpertName = appointment.ExpertName,
                            ExpertEmail = appointment.ExpertEmail,
                            AppointmentDate = appointment.AppointmentDate,
                            StartTime = appointment.StartTime,
                            EndTime = appointment.EndTime,
                            PaymentDate = pay.PaymentDate,
                            UpdatedAt = pay.UpdatedAt
                        };
                        paymentManagerUserDtos.Add(paymentManagerUserDto);
                    }
                }
            }

            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.SuccessReturn(paymentManagerUserDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PaymentManagerUserDto>>.InternalServerError(e.Message);
        }
    }
}