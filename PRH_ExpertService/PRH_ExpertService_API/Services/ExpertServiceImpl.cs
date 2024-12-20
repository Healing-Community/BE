using Grpc.Core;
using ExpertService.gRPC;
using Application.Interfaces.Services;
using Application.Interfaces.Repository;

namespace PRH_ExpertService_API.Services
{
    public class ExpertServiceImpl(
        IEmailService emailService,
        IAppointmentRepository appointmentRepository,
        IExpertAvailabilityRepository availabilityRepository) : ExpertService.gRPC.ExpertService.ExpertServiceBase
    {
        public override async Task<PaymentSuccessResponse> PaymentSuccess(PaymentSuccessRequest request, ServerCallContext context)
        {
            try
            {
                // Lấy thông tin lịch hẹn
                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return new PaymentSuccessResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy lịch hẹn."
                    };
                }

                // Sau khi thanh toán thành công, chuyển trạng thái Appointment sang Scheduled (1)
                appointment.Status = 1; // Scheduled
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                // ExpertAvailability = Booked (2) vẫn giữ nguyên
                var availability = await availabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                if (availability != null)
                {
                    availability.Status = 2; // Booked
                    availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);
                }

                // Gửi email cho user
                await emailService.SendAppointmentConfirmationEmailAsync(
                    appointment.UserEmail,
                    $"{appointment.AppointmentDate:dd/MM/yyyy} từ {appointment.StartTime} đến {appointment.EndTime}",
                    appointment.MeetLink
                );

                // Gửi email cho expert
                await emailService.SendAppointmentNotificationToExpertAsync(
                    appointment.ExpertEmail,
                    $"{appointment.AppointmentDate:dd/MM/yyyy} từ {appointment.StartTime} đến {appointment.EndTime}",
                    appointment.MeetLink
                );

                return new PaymentSuccessResponse
                {
                    Success = true,
                    Message = "Đã xác nhận đặt lịch thành công và chuyển sang trạng thái Scheduled."
                };
            }
            catch (Exception ex)
            {
                return new PaymentSuccessResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }
    }
}
