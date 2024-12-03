using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.BookAppointment
{
    public class BookAppointmentCommandHandler(
        IExpertAvailabilityRepository availabilityRepository,
        IAppointmentRepository appointmentRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<BookAppointmentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                // Lấy email của người dùng
                var userEmail = Authentication.GetUserEmailFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userEmail))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy email người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                var availability = await availabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null || availability.Status != 0) // Check trạng thái Available
                {
                    response.Success = false;
                    response.Message = "Lịch trống không tồn tại hoặc không khả dụng.";
                    response.StatusCode = 404;
                    return response;
                }

                // Lấy email của chuyên gia
                var expertProfile = await expertProfileRepository.GetByIdAsync(availability.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }
                var expertEmail = expertProfile.Email;

                // Đổi trạng thái của lịch trống sang PendingPayment
                availability.Status = 1; // PendingPayment
                availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                // Tạo liên kết Jitsi Meet
                var meetingRoomName = $"{Ulid.NewUlid()}-{availability.ExpertProfileId}";
                var meetingUrl = $"https://meet.jit.si/{meetingRoomName}";

                // Tạo mới một bản ghi Appointment
                var appointment = new Appointment
                {
                    AppointmentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    UserEmail = userEmail,
                    ExpertProfileId = availability.ExpertProfileId,
                    ExpertEmail = expertEmail,
                    ExpertAvailabilityId = availability.ExpertAvailabilityId,
                    AppointmentDate = availability.AvailableDate,
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    Status = 1, // PendingPayment
                    MeetLink = meetingUrl,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await appointmentRepository.Create(appointment);

                // Trả về thông tin để tiếp tục thanh toán
                response.Success = true;
                response.Data = appointment.AppointmentId;
                response.StatusCode = 200;
                response.Message = "Yêu cầu đặt lịch đã được ghi nhận. Vui lòng hoàn tất thanh toán để xác nhận lịch hẹn.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi đặt lịch hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
