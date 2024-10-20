﻿using Application.Commons.Request.Comment;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;

namespace PRH_NotificationService_API.Consumer
{
    public class CommentServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<CommentRequestCreatedMessage>
    {
        public async Task Consume(ConsumeContext<CommentRequestCreatedMessage> context)
        {
            var commentRequest = context.Message;

            var notificationType = await notificationTypeRepository.GetByNameAsync("Comment") ?? throw new Exception("Notification type not found");
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = commentRequest.UserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {commentRequest.UserId} đã bình luận: {commentRequest.Content} trên bài viết {commentRequest.PostId}",
                CreatedAt = commentRequest.CommentedDate,
                UpdatedAt = commentRequest.CommentedDate,
                IsRead = false
            };

            await notificationRepository.CreateNotificationsAsync([notification]);
        }
    }
}