using Grpc.Core;
using ExpertService.gRPC;
using Application.Interfaces.Services;
using Application.Interfaces.Repository;
using Application.Services;

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
                // 1. Lấy thông tin lịch hẹn
                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return new PaymentSuccessResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy lịch hẹn."
                    };
                }

                if (request.IsSuccess)
                {
                    // Xử lý khi thanh toán thành công
                    // 2. Cập nhật trạng thái lịch hẹn thành 'Đã thanh toán'
                    appointment.Status = 2; // Đã thanh toán
                    appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await appointmentRepository.Update(appointment.AppointmentId, appointment);

                    // 3. Cập nhật trạng thái lịch trống thành 'Đã đặt hẹn'
                    var availability = await availabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                    if (availability != null)
                    {
                        availability.Status = 2; // Đã đặt hẹn
                        availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                        await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);
                    }

                    // 4. Chuẩn bị thông tin cho email
                    string appointmentTime = $"{appointment.AppointmentDate:dd/MM/yyyy} từ {appointment.StartTime} đến {appointment.EndTime}";
                    string meetingLink = appointment.MeetLink;
                    string userEmail = appointment.UserEmail;
                    string expertEmail = appointment.ExpertEmail;

                    // 5. Gửi email xác nhận cho người dùng
                    await emailService.SendAppointmentConfirmationEmailAsync(
                        userEmail,
                        appointmentTime,
                        meetingLink
                    );

                    // 6. Gửi email thông báo cho chuyên gia
                    await emailService.SendAppointmentNotificationToExpertAsync(
                        expertEmail,
                        appointmentTime,
                        meetingLink
                    );

                    // 7. Trả về kết quả thành công
                    return new PaymentSuccessResponse
                    {
                        Success = true,
                        Message = "Đã xử lý thanh toán thành công và gửi email xác nhận."
                    };
                }
                else
                {
                    // Xử lý khi thanh toán thất bại hoặc bị hủy
                    // 2. Cập nhật trạng thái lịch hẹn thành 'Đã hủy'
                    appointment.Status = 3; // Đã hủy
                    appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await appointmentRepository.Update(appointment.AppointmentId, appointment);

                    // 3. Cập nhật trạng thái lịch trống thành 'Còn trống'
                    var availability = await availabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                    if (availability != null)
                    {
                        availability.Status = 1; // Còn trống
                        availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                        await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);
                    }

                    // 4. Gửi email thông báo hủy lịch hẹn cho người dùng
                    await emailService.SendAppointmentCancellationEmailAsync(
                        appointment.UserEmail,
                        $"{appointment.AppointmentDate:dd/MM/yyyy} từ {appointment.StartTime} đến {appointment.EndTime}"
                    );

                    // 5. Trả về kết quả thành công
                    return new PaymentSuccessResponse
                    {
                        Success = true,
                        Message = "Đã xử lý hủy thanh toán và gửi email thông báo."
                    };
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return new PaymentSuccessResponse
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }
    }
}
