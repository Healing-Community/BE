﻿using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer
{
    public class FollowServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<FollowMessage>
    {
        public async Task Consume(ConsumeContext<FollowMessage> context)
        {
            var followRequest = context.Message;

            var notificationType = await notificationTypeRepository.GetByIdAsync("06");
            var notification = new Notification
            {
                NotificationId = Ulid.NewUlid().ToString(),
                UserId = followRequest.FollowedUserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {followRequest.FollowingUserName} đã theo dõi {followRequest.FollowedUserName}",
                CreatedAt = followRequest.FollowDate,
                UpdatedAt = followRequest.FollowDate,
                IsRead = false
            };

            await notificationRepository.CreateNotificationsAsync(new List<Notification> { notification });
        }
    }
}
