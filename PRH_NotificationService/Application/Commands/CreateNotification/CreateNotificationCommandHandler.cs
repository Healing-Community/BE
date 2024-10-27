using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IRequestHandler<CreateNotificationCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var notificationType = await notificationTypeRepository.GetByIdAsync(request.NotificationTypeId);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy loại thông báo với ID '{request.NotificationTypeId}'. Vui lòng kiểm tra lại.";
                    response.StatusCode = 404;
                    return response;
                }

                var isSubscribed = await notificationRepository.GetUserNotificationPreferenceAsync(request.UserId, notificationType.NotificationTypeId);
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
                    UserId = request.UserId,
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
