using Application.Commands.NotifyFollowers;
using Application.Commons;
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
        private readonly IUserRepository _userRepository;

        public NotifyFollowersCommandHandler(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<string>> Handle(NotifyFollowersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var followers = await _userRepository.GetFollowersAsync(request.UserId);
                if (followers == null || !followers.Any())
                {
                    response.Success = false;
                    response.Message = "No followers found.";
                    return response;
                }

                var notificationType = await _notificationRepository.GetNotificationTypeByEnum(NotificationTypeEnum.NewPostByFollowedUser);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Notification type not found.";
                    return response;
                }

                var notifications = new List<Domain.Entities.Notification>();
                var tasks = followers.Select(async follower =>
                {
                    var preference = await _notificationRepository.GetUserNotificationPreferenceAsync(follower.UserId, notificationType.NotificationTypeId);
                    if (preference)
                    {
                        notifications.Add(new Domain.Entities.Notification
                        {
                            UserId = follower.UserId,
                            NotificationTypeId = notificationType.NotificationTypeId,
                            Message = $"New post titled '{request.PostTitle}' by user {request.UserId}",
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
                response.Message = "Notifications sent to followers.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to notify followers.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
