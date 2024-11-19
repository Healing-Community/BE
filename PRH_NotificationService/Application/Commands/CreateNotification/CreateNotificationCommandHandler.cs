using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler(INotificationRepository notificationRepository,
        INotificationTypeRepository notificationTypeRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateNotificationCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
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

                var notificationType = await notificationTypeRepository.GetByIdAsync(request.NotificationTypeId);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy loại thông báo với ID '{request.NotificationTypeId}'. Vui lòng kiểm tra lại.";
                    response.StatusCode = 404;
                    return response;
                }

                var isSubscribed = await notificationRepository.GetUserNotificationPreferenceAsync(userId, notificationType.NotificationTypeId);
                if (!isSubscribed)
                {
                    response.Success = false;
                    response.Message = "Người dùng chưa đăng ký nhận loại thông báo này. Vui lòng kiểm tra cài đặt thông báo của bạn.";
                    response.StatusCode = 404;
                    return response;
                }

                var notification = new Domain.Entities.Notification
                {
                    NotificationId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    NotificationTypeId = notificationType.NotificationTypeId,
                    Message = request.Message,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await notificationRepository.Create(notification);

                response.Success = true;
                response.Message = "Thông báo đã được tạo thành công.";
                response.Data = notification.NotificationId.ToString();
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể tạo thông báo. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
