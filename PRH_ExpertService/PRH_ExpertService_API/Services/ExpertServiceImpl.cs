using Grpc.Core;
using ExpertService.gRPC;
using Application.Interfaces.Services;
using Application.Interfaces.Repository;

namespace PRH_ExpertService_API.Services
{
    public class ExpertServiceImpl(
        IEmailService emailService,
        IAppointmentRepository appointmentRepository,
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IExpertProfileRepository expertProfileRepository) : ExpertService.gRPC.ExpertService.ExpertServiceBase
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
                var availability = await expertAvailabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                if (availability != null)
                {
                    availability.Status = 2; // Booked
                    availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);
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

        public override async Task<GetAppointmentDetailsResponse> GetAppointmentDetails(GetAppointmentDetailsRequest request, ServerCallContext context)
        {
            try
            {
                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return new GetAppointmentDetailsResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy AppointmentId = {request.AppointmentId}"
                    };
                }

                var expert = await expertProfileRepository.GetByIdAsync(appointment.ExpertProfileId);
                if (expert == null)
                {
                    return new GetAppointmentDetailsResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy ExpertId = {appointment.ExpertProfileId}"
                    };
                }

                return new GetAppointmentDetailsResponse
                {
                    Success = true,
                    Message = "Lấy thông tin Appointment thành công.",
                    ExpertName = expert.Fullname,
                    ExpertEmail = appointment.ExpertEmail,
                    AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                    StartTime = appointment.StartTime.ToString(),
                    EndTime = appointment.EndTime.ToString()
                };
            }
            catch (Exception ex)
            {
                return new GetAppointmentDetailsResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }
    }
}
