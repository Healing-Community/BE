using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage.Reaction;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer;

public class ReactionServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<ReactionRequestCreatedMessage>
{
    public async Task Consume(ConsumeContext<ReactionRequestCreatedMessage> context)
    {
        var reactionRequest = context.Message;

        // Lấy thông tin loại thông báo
        var notificationType = await notificationTypeRepository.GetByIdAsync("10")
                               ?? throw new Exception("Notification type not found");

        // Tạo thông báo
        var notification = new Notification
        {
            NotificationId = Ulid.NewUlid().ToString(),
            UserId = reactionRequest.UserId,
            PostId = reactionRequest.PostId,
            NotificationTypeId = notificationType.NotificationTypeId,
            Message = $"Người dùng {reactionRequest.UserName} đã {reactionRequest.ReactionTypeIcon} với bài viết {reactionRequest.Title}",
            CreatedAt = reactionRequest.ReactionDate,
            UpdatedAt = reactionRequest.ReactionDate,
            IsRead = false
        };

        // Lưu thông báo vào cơ sở dữ liệu
        await notificationRepository.CreateNotificationsAsync(new List<Notification> { notification });
    }
}