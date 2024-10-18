﻿using Application.Commons.Request.Post;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;

namespace PRH_NotificationService_API.Consumer
{
    public class PostServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<PostingRequestCreatedMessage>
    {
        public async Task Consume(ConsumeContext<PostingRequestCreatedMessage> context)
        {
            var postingRequest = context.Message;

            //await Console.Out.WriteLineAsync($"Received message: {postingRequest.Tittle}");

            var notificationType = await notificationTypeRepository.GetByNameAsync("NewPostByFollowedUser") ?? throw new Exception("Notification type not found");
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = postingRequest.UserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = postingRequest.Tittle ?? string.Empty,
                CreatedAt = postingRequest.PostedDate,
                UpdatedAt = postingRequest.PostedDate,
                IsRead = false
            };

            //await Console.Out.WriteLineAsync($"Creating notification for UserId: {postingRequest.UserId}");

            await notificationRepository.CreateNotificationsAsync([notification]);

            //await Console.Out.WriteLineAsync("Notification created successfully");
        }
    }
}
