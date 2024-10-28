using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

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

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Lịch hẹn không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                if (appointment.Status == 1 || appointment.Status == 2)
                {
                    response.Success = false;
                    response.Message = "Không thể cập nhật lịch hẹn đã hoàn thành hoặc bị hủy.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Success = false;
                    response.Message = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.NewAppointmentDate < DateTime.UtcNow.Date ||
                   (request.NewAppointmentDate == DateTime.UtcNow.Date && request.NewEndTime <= DateTime.UtcNow.TimeOfDay))
                {
                    response.Success = false;
                    response.Message = "Ngày và thời gian của lịch hẹn phải là trong tương lai.";
                    response.StatusCode = 400;
                    return response;
                }

                var overlappingAppointments = await appointmentRepository.GetOverlappingAppointmentsAsync(
                    appointment.ExpertProfileId, request.NewAppointmentDate, request.NewStartTime, request.NewEndTime);

                if (overlappingAppointments.Any(a => a.AppointmentId != appointment.AppointmentId))
                {
                    response.Success = false;
                    response.Message = "Thời gian hẹn bị trùng lặp với một lịch hẹn khác.";
                    response.StatusCode = 400;
                    return response;
                }

                appointment.AppointmentDate = request.NewAppointmentDate;
                appointment.StartTime = request.NewStartTime;
                appointment.EndTime = request.NewEndTime;
                appointment.UpdatedAt = DateTime.UtcNow;

                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Cập nhật lịch hẹn thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật lịch hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
