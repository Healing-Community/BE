using Application.Commons.Request.Reaction;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer
{
    public class ReactionServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<ReactionRequestCreatedMessage>
    {
        public async Task Consume(ConsumeContext<ReactionRequestCreatedMessage> context)
        {
            var reactionRequest = context.Message;

            var notificationType = await notificationTypeRepository.GetByIdAsync("10") ?? throw new Exception("Notification type not found");
            var notification = new Notification
            {
                NotificationId = Ulid.NewUlid().ToString(),
                UserId = reactionRequest.UserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {reactionRequest.UserId} đã phản ứng với bài viết {reactionRequest.PostId} bằng loại phản ứng {reactionRequest.ReactionTypeId}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await notificationRepository.CreateNotificationsAsync([notification]);
        }
    }
}
