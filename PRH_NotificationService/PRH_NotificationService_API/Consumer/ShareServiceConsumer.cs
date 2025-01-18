using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage.Share;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer;

public class ShareServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<ShareMessage>
{
    public async Task Consume(ConsumeContext<ShareMessage> context)
    {
        var shareRequest = context.Message;

        // Lấy thông tin loại thông báo
        var notificationType = await notificationTypeRepository.GetByIdAsync("11")
                               ?? throw new Exception("Notification type not found");

        // Tạo thông báo gửi đến người sở hữu bài viết
        var notification = new Notification
        {
            NotificationId = Ulid.NewUlid().ToString(),
            UserId = shareRequest.UserPostId,
            PostId = shareRequest.PostId,
            NotificationTypeId = notificationType.NotificationTypeId,
            Message = $"{shareRequest.UserName} đã chia sẻ bài viết của bạn trên nền tảng {shareRequest.Platform}.",
            CreatedAt = shareRequest.ShareDate,
            UpdatedAt = shareRequest.ShareDate,
            IsRead = false
        };

        // Lưu thông báo vào cơ sở dữ liệu
        await notificationRepository.CreateNotificationsAsync(new List<Notification> { notification });
    }
}
