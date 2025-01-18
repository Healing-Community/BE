using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage.Post;
using Domain.Entities;
using MassTransit;
using NUlid;

namespace PRH_NotificationService_API.Consumer;

public class PostServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<PostingRequestCreatedMessage>
{
    public async Task Consume(ConsumeContext<PostingRequestCreatedMessage> context)
    {
        var postingRequest = context.Message;

        // Lấy thông tin loại thông báo
        var notificationType = await notificationTypeRepository.GetByIdAsync("09")
                               ?? throw new Exception("Notification type not found");

        // Kiểm tra danh sách FollowersId
        if (postingRequest.FollowersId != null && postingRequest.FollowersId.Any())
        {
            // Tạo thông báo cho từng follower
            var notifications = postingRequest.FollowersId.Select(followerId => new Notification
            {
                NotificationId = Ulid.NewUlid().ToString(),
                UserId = followerId,
                PostId = postingRequest.PostingRequestId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {postingRequest.UserName} đã đăng bài viết với tiêu đề: {postingRequest.Tittle}",
                CreatedAt = postingRequest.PostedDate,
                UpdatedAt = postingRequest.PostedDate,
                IsRead = false
            }).ToList();

            // Lưu danh sách thông báo vào cơ sở dữ liệu
            await notificationRepository.CreateNotificationsAsync(notifications);
        }
    }
}
