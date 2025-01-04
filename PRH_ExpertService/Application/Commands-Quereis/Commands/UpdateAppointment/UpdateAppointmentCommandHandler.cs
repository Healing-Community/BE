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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateAppointmentCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
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

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lịch hẹn không tồn tại.",
                        Field = "AppointmentId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.Status == 1 || appointment.Status == 2) // 1 = Completed, 2 = Cancelled
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không thể cập nhật lịch hẹn đã hoàn thành hoặc bị hủy.",
                        Field = "Status"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewEndTime <= request.NewStartTime)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Thời gian kết thúc phải sau thời gian bắt đầu.",
                        Field = "EndTime"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.NewAppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) ||
                   (request.NewAppointmentDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)) && request.NewEndTime <= TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7))))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Ngày và thời gian của lịch hẹn phải là trong tương lai.",
                        Field = "AppointmentDate"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var overlappingAppointments = await appointmentRepository.GetOverlappingAppointmentsAsync(
                    appointment.ExpertProfileId, request.NewAppointmentDate, request.NewStartTime, request.NewEndTime);

                if (overlappingAppointments.Any(a => a.AppointmentId != appointment.AppointmentId))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Thời gian hẹn bị trùng lặp với một lịch hẹn khác.",
                        Field = "TimeRange"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
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
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật lịch hẹn.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
