using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.RateExpert
{
    public class RateExpertCommandHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<RateExpertCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(RateExpertCommand request, CancellationToken cancellationToken)
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
                        Message = "Không tìm thấy lịch hẹn.",
                        Field = "AppointmentId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.UserId != userId)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Người dùng không có quyền đánh giá lịch hẹn này.",
                        Field = "UserId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.Status != 3) // 3 = Completed
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Chỉ có thể đánh giá sau khi tư vấn hoàn thành.",
                        Field = "AppointmentStatus"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Đánh giá phải nằm trong khoảng từ 1 đến 5.",
                        Field = "Rating"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật đánh giá
                appointment.Rating = request.Rating;
                appointment.Comment = request.Comment;
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Đánh giá thành công.";
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi đánh giá.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
