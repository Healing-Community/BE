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

                var isSubscribed = await _notificationRepository.GetUserNotificationPreferenceAsync(request.UserId, notificationType.NotificationTypeId);
                if (!isSubscribed)
                {
                    response.Success = false;
                    response.Message = "Người dùng không đăng ký loại thông báo này.";
                    response.StatusCode = 404;
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
                response.Message = "Thông báo đã được tạo thành công.";
                response.Data = notification.NotificationId.ToString();
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể tạo thông báo.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
