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
            IExpertAvailabilityRepository expertAvailabilityRepository,
            IAppointmentRepository appointmentRepository,
            IExpertProfileRepository expertProfileRepository,
            IHttpContextAccessor httpContextAccessor) : IRequestHandler<BookAppointmentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
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

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null || availability.Status != 0)
                {
                    response.Success = false;
                    response.Message = "Lịch trống không tồn tại hoặc không khả dụng.";
                    response.StatusCode = 404;
                    return response;
                }

                if (availability.AvailableDate < DateTime.UtcNow.Date ||
                    (availability.AvailableDate == DateTime.UtcNow.Date && availability.EndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Success = false;
                    response.Message = "Thời gian của lịch trống đã qua hoặc không hợp lệ.";
                    response.StatusCode = 400;
                    return response;
                }

                var expertProfile = await expertProfileRepository.GetByIdAsync(availability.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy thông tin chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                if (expertProfile.Status != 1)
                {
                    response.Success = false;
                    response.Message = "Chuyên gia không khả dụng để đặt lịch.";
                    response.StatusCode = 400;
                    return response;
                }

                availability.Status = 1;
                availability.UpdatedAt = DateTime.UtcNow;
                await expertAvailabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                string meetingLink = $"https://meet.example.com/session/{Ulid.NewUlid()}";

                var appointment = new Appointment
                {
                    AppointmentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    ExpertProfileId = availability.ExpertProfileId,
                    ExpertAvailabilityId = availability.ExpertAvailabilityId,
                    AppointmentDate = availability.AvailableDate,
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    MeetLink = meetingLink,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await appointmentRepository.Create(appointment);

                response.Success = true;
                response.Message = "Lịch hẹn đã được tạo thành công.";
                response.Data = appointment.AppointmentId;
                response.StatusCode = 200;
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
