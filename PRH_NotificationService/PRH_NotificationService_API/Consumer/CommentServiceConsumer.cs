﻿using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage.Comment;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer;

public class CommentServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<CommentRequestCreatedMessage>
{
    public async Task Consume(ConsumeContext<CommentRequestCreatedMessage> context)
    {
        var commentRequest = context.Message;

        // Lấy thông tin loại thông báo
        var notificationType = await notificationTypeRepository.GetByIdAsync("08")
                               ?? throw new Exception("Notification type not found");

        var notification = new Notification
        {
            NotificationId = Ulid.NewUlid().ToString(),
            UserId = commentRequest.UserPostId,
            PostId = commentRequest.PostId,
            NotificationTypeId = notificationType.NotificationTypeId,
            Message = $"Người dùng {commentRequest.UserName} đã bình luận {commentRequest.Content} trên bài viết của bạn.",
            CreatedAt = commentRequest.CommentedDate,
            UpdatedAt = commentRequest.CommentedDate,
            IsRead = false
        };

        // Lưu thông báo vào cơ sở dữ liệu
        await notificationRepository.CreateNotificationsAsync(new List<Notification> { notification });
    }
}
