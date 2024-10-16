using Application.Commands.NotifyFollowers;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;

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
                var userId = Authentication.GetUserIdFromHttpContext(request.NotifyFollowersRequestDto.Context ?? throw new InvalidOperationException());
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("User ID cannot be null or empty.");
                }

                var parsedUserId = Guid.Parse(userId);

                var notificationType = await _notificationRepository.GetNotificationTypeByEnum(NotificationTypeEnum.NewPostByFollowedUser);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy loại thông báo.";
                    response.StatusCode = 404;
                    return response;
                }

                var followerIds = request.NotifyFollowersRequestDto.Followers.Select(f => f.UserId).ToList();

                // Lấy tất cả các preferences của các followers một lần
                var userPreferences = await _notificationRepository.GetUserNotificationPreferencesAsync(followerIds, notificationType.NotificationTypeId);

                var notifications = new List<Domain.Entities.Notification>();

                foreach (var preference in userPreferences)
                {
                    notifications.Add(new Domain.Entities.Notification
                    {
                        UserId = preference.UserId,
                        NotificationTypeId = notificationType.NotificationTypeId,
                        Message = $"Bài viết mới có tiêu đề '{request.NotifyFollowersRequestDto.PostTitle}' bởi người dùng {parsedUserId}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
                }

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
