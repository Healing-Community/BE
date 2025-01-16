using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage.Share;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer
{
    public class ShareServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<ShareMessage>
    {
        public async Task Consume(ConsumeContext<ShareMessage> context)
        {
            var shareRequest = context.Message;

            var notificationType = await notificationTypeRepository.GetByIdAsync("11") ?? throw new Exception("Notification type not found");
            var notification = new Notification
            {
                NotificationId = Ulid.NewUlid().ToString(),
                UserId = shareRequest.UserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {shareRequest.UserId} đã chia sẻ bài viết {shareRequest.PostId}",
                CreatedAt = shareRequest.ShareDate,
                UpdatedAt = shareRequest.ShareDate,
                IsRead = false
            };

            await notificationRepository.CreateNotificationsAsync([notification]);
        }
    }
}
