using Application.Commands.NotifyFollowers;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Notification
{
    public class NotifyFollowersCommandHandler : IRequestHandler<NotifyFollowersCommand, BaseResponse<string>>
    {
        private readonly INotificationRepository _notificationRepository;

        public NotifyFollowersCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<string>> Handle(NotifyFollowersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                // Extract user ID from the token
                var userId = Authentication.GetUserIdFromHttpContext(request.NotifyFollowersRequestDto.Context ?? throw new InvalidOperationException());
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("User ID cannot be null or empty.");
                }

                var parsedUserId = Guid.Parse(userId);

                // Fetch notification type
                var notificationType = await _notificationRepository.GetNotificationTypeByEnum(NotificationTypeEnum.NewPostByFollowedUser);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy loại thông báo.";
                    response.StatusCode = 404;
                    return response;
                }

                // Create notifications for followers
                var notifications = new List<Domain.Entities.Notification>();
                var tasks = request.NotifyFollowersRequestDto.Followers.Select(async follower =>
                {
                    var preference = await _notificationRepository.GetUserNotificationPreferenceAsync(follower.UserId, notificationType.NotificationTypeId);
                    if (preference)
                    {
                        notifications.Add(new Domain.Entities.Notification
                        {
                            UserId = follower.UserId,
                            NotificationTypeId = notificationType.NotificationTypeId,
                            Message = $"Bài viết mới có tiêu đề '{request.NotifyFollowersRequestDto.PostTitle}' bởi người dùng {parsedUserId}",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsRead = false
                        });
                    }
                });

                await Task.WhenAll(tasks);

                if (notifications.Any())
                {
                    await _notificationRepository.CreateNotificationsAsync(notifications);
                }

                response.Success = true;
                response.Message = "Thông báo đã được gửi đến người theo dõi.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể thông báo cho người theo dõi.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
