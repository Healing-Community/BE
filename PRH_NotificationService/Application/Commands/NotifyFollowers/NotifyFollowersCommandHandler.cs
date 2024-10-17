using Application.Commands.NotifyFollowers;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.Notification
{
    public class NotifyFollowersCommandHandler : IRequestHandler<NotifyFollowersCommand, BaseResponse<string>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationTypeRepository _notificationTypeRepository;

        public NotifyFollowersCommandHandler(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository)
        {
            _notificationRepository = notificationRepository;
            _notificationTypeRepository = notificationTypeRepository;
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
                var notificationType = await _notificationTypeRepository.GetByIdAsync(request.NotificationTypeId);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy loại thông báo.";
                    response.StatusCode = 404;
                    return response;
                }

                var userPreferences = await _notificationRepository.GetUserNotificationPreferencesAsync(new List<Guid>(), notificationType.NotificationTypeId);

                var notifications = new List<Domain.Entities.Notification>();

                foreach (var preference in userPreferences)
                {
                    notifications.Add(new Domain.Entities.Notification
                    {
                        UserId = preference.UserId,
                        NotificationTypeId = notificationType.NotificationTypeId,
                        Message = $"Bài viết mới có tiêu đề '{request.PostTitle}' bởi người dùng {request.UserId}",
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
