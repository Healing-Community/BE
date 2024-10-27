using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<MarkNotificationAsReadCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var notification = await notificationRepository.GetByIdAsync(request.NotificationId);
                if (notification == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy thông báo với ID '{request.NotificationId}'. Vui lòng kiểm tra lại.";
                    response.StatusCode = 404;
                    return response;
                }

                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;

                await notificationRepository.Update(notification.NotificationId, notification);

                response.Success = true;
                response.Message = "Thông báo đã được đánh dấu là đã đọc.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể đánh dấu thông báo là đã đọc. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
