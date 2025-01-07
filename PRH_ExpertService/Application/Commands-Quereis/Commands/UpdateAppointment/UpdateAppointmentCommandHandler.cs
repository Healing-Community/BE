using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.UpdateAppointment
{
    public class UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateAppointmentCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
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
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lịch hẹn không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.Status == 1 || appointment.Status == 2) // 1 = Completed, 2 = Cancelled
                {
                    response.Success = false;
                    response.Errors.Add("Không thể cập nhật lịch hẹn đã hoàn thành hoặc bị hủy.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Success = false;
                    response.Errors.Add("Thời gian kết thúc phải sau thời gian bắt đầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewAppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) ||
                   (request.NewAppointmentDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) && request.NewEndTime <= TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7))))
                {
                    response.Success = false;
                    response.Errors.Add("Ngày và thời gian của lịch hẹn phải là trong tương lai.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var overlappingAppointments = await appointmentRepository.GetOverlappingAppointmentsAsync(
                    appointment.ExpertProfileId, request.NewAppointmentDate, request.NewStartTime, request.NewEndTime);

                if (overlappingAppointments.Any(a => a.AppointmentId != appointment.AppointmentId))
                {
                    response.Success = false;
                    response.Errors.Add("Thời gian hẹn bị trùng lặp với một lịch hẹn khác.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật lịch hẹn
                appointment.AppointmentDate = request.NewAppointmentDate;
                appointment.StartTime = request.NewStartTime;
                appointment.EndTime = request.NewEndTime;
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Cập nhật lịch hẹn thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
