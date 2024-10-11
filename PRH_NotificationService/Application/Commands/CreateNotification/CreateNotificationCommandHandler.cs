using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.Notification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, BaseResponse<string>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationTypeRepository _notificationTypeRepository;

        public CreateNotificationCommandHandler(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository)
        {
            _notificationRepository = notificationRepository;
            _notificationTypeRepository = notificationTypeRepository;
        }

        public async Task<BaseResponse<string>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var notificationType = await _notificationTypeRepository.GetByNameAsync(request.NotificationType.ToString());
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = "Invalid notification type.";
                    return response;
                }

                var isSubscribed = await _notificationRepository.GetUserNotificationPreferenceAsync(request.UserId, notificationType.NotificationTypeId);
                if (!isSubscribed)
                {
                    response.Success = false;
                    response.Message = "User is not subscribed to this notification type.";
                    return response;
                }

                var notification = new Domain.Entities.Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = request.UserId,
                    NotificationTypeId = notificationType.NotificationTypeId,
                    Message = request.Message,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationRepository.Create(notification);

                response.Success = true;
                response.Message = "Notification created successfully.";
                response.Data = notification.NotificationId.ToString();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to create notification.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
