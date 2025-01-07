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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<BookAppointmentCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var userEmail = Authentication.GetUserEmailFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userEmail))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không tìm thấy email người dùng.",
                        Field = "UserEmail"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var availability = await availabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null || availability.Status != 0) // 0 = Available
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lịch trống không tồn tại hoặc không khả dụng.",
                        Field = "ExpertAvailabilityId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var expertProfile = await expertProfileRepository.GetByIdAsync(availability.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không tìm thấy hồ sơ chuyên gia.",
                        Field = "ExpertProfileId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
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

                // Tạo Appointment
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
                    Status = 0, // PendingPayment
                    MeetLink = meetingUrl,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await appointmentRepository.Create(appointment);

                response.Success = true;
                response.Data = appointment.AppointmentId;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Yêu cầu đặt lịch đã được ghi nhận. Vui lòng hoàn tất thanh toán để xác nhận lịch hẹn.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Chi tiết lỗi: {ex.Message}",
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi đặt lịch hẹn.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
