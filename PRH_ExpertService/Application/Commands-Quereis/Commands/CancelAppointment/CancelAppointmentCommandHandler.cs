using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using PaymentExpertService;

namespace Application.Commands.CancelAppointment
{
    public class CancelAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository, IGrpcHelper grpcHelper, IHttpContextAccessor accessor,
        IExpertAvailabilityRepository availabilityRepository, IExpertProfileRepository expertProfileRepository) : IRequestHandler<CancelAppointmentCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userRole = accessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return BaseResponse<bool>.NotFound("Lịch hẹn không tồn tại.");
                }
                // Kiểm tra trạng thái. Nếu Completed(3) hoặc Cancelled(2) thì không thể hủy.
                if (appointment.Status == 3 || appointment.Status == 2)
                {
                    return BaseResponse<bool>.BadRequest("Lịch hẹn đã hoàn thành hoặc đã bị hủy trước đó.");
                }

                // Kiểm tra lịch hẹn với thời gian hiện tại xem đã qua 24h kể từ lúc hẹn chưa.
                // Assuming AppointmentDate is a date-only property (e.g., DateOnly in C# 10 or a DateTime with time truncated)
                var appointmentDateTime = appointment.AppointmentDate.ToDateTime(TimeOnly.MinValue).AddHours(24);
                if (DateTime.UtcNow.AddHours(7) > appointmentDateTime)
                {
                    return BaseResponse<bool>.BadRequest("Không thể hủy lịch hẹn sau 24h kể từ thời gian hẹn.");
                }

                appointment.Status = 2; // Cancelled
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                var availability = await availabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);

                if (availability == null)
                {
                    return BaseResponse<bool>.NotFound("Không tìm thấy thông tin lịch hẹn.");
                }
                availability.Status = 0; // Available
                await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                // Trừ điểm rating của Expert nếu họ hủy lịch hẹn
                if (userRole == "Expert")
                {
                    var expertProfile = await expertProfileRepository.GetByIdAsync(appointment.ExpertProfileId);
                    if (expertProfile != null)
                    {
                        expertProfile.AverageRating -= 0.1M; // Trừ 0.1 điểm

                        // Đảm bảo điểm rating nằm trong khoảng từ 1 đến 5
                        expertProfile.AverageRating = Math.Clamp(expertProfile.AverageRating, 1, 5);

                        await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);
                    }
                }

                var status = userRole == "User" ? 5 : userRole == "Expert" ? 6 : throw new ArgumentException("Invalid role", nameof(userRole));
                // Grpc call to Payment service to refund the payment
                var reply = await grpcHelper.ExecuteGrpcCallAsync<PaymentService.PaymentServiceClient, UpdatePaymentAppointmentRequest, UpdatePaymentAppointResponse>(
                    "PaymentService",
                    async client => await client.UpdatePaymentAsync(new UpdatePaymentAppointmentRequest { AppointmentId = appointment.AppointmentId, Status = status })
                );

                if (reply == null)
                {
                    return BaseResponse<bool>.InternalServerError("Có lỗi xảy ra khi hủy lịch hẹn.");
                }

                if (userRole == "Expert")
                {
                    // Chưa có trừ điểm rating của Expert ở đây
                    return BaseResponse<bool>.SuccessReturn(reply.IsSucess, "Hủy lịch hẹn thành công điểm sẽ bị trừ n điểm.");
                }
                else
                {
                    return BaseResponse<bool>.SuccessReturn(reply.IsSucess, "Hủy lịch hẹn thành công tiền sẽ được hoàn trả trong vòng 1-5 ngày.");
                }
            }
            catch (Exception ex)
            {
                return BaseResponse<bool>.InternalServerError(ex.Message);
            }
        }
    }
}
