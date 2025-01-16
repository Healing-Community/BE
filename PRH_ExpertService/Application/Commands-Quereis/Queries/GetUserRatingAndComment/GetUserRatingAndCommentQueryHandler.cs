using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetUserRatingAndComment
{
    public class GetUserRatingAndCommentQueryHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserRatingAndCommentQuery, BaseResponse<UserRatingAndCommentDto>>
    {
        public async Task<BaseResponse<UserRatingAndCommentDto>> Handle(GetUserRatingAndCommentQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<UserRatingAndCommentDto>
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
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = 401;
                    return response;
                }

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null || appointment.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Lịch hẹn không tồn tại hoặc không thuộc về người dùng hiện tại.";
                    response.StatusCode = 200; // Trả về 200 để tránh lỗi 404
                    return response;
                }

                if (!appointment.Rating.HasValue)
                {
                    response.Success = false;
                    response.Message = "Lịch hẹn chưa được đánh giá.";
                    response.StatusCode = 200; // Trả về 200 thay vì 404
                    return response;
                }

                response.Success = true;
                response.Data = new UserRatingAndCommentDto
                {
                    Rating = appointment.Rating.Value,
                    Comment = appointment.Comment
                };
                response.StatusCode = 200;
                response.Message = "Lấy thông tin đánh giá và bình luận thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin đánh giá và bình luận.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
