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
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.",
                        Field = "HttpContext"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lịch hẹn không tồn tại.",
                        Field = "AppointmentId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                if (appointment.Status == 1 || appointment.Status == 2)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không thể cập nhật lịch hẹn đã hoàn thành hoặc bị hủy.",
                        Field = "Status"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
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
                    response.StatusCode = 400;
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
                    response.StatusCode = 400;
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
                    response.StatusCode = 400;
                    return response;
                }

                appointment.AppointmentDate = request.NewAppointmentDate;
                appointment.StartTime = request.NewStartTime;
                appointment.EndTime = request.NewEndTime;
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
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
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
