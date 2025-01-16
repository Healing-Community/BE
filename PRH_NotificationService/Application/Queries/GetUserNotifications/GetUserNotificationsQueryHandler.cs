using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler(INotificationRepository notificationRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserNotificationsQuery, BaseResponse<List<NotificationDto>>>
    {
        public async Task<BaseResponse<List<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<NotificationDto>>
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

                var notifications = await notificationRepository.GetNotificationsByUserAsync(userId);

                response.Success = true;
                response.Data = notifications.Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    NotificationTypeId = n.NotificationTypeId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList();
                response.StatusCode = 200;
                response.Message = "Lấy thông báo thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông báo. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
