using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, BaseResponse<string>>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
                if (notification == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy thông báo.";
                    response.StatusCode = 404;
                    return response;
                }

                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;

                await _notificationRepository.Update(notification.NotificationId, notification);

                response.Success = true;
                response.Message = "Thông báo đã được đánh dấu là đã đọc.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể đánh dấu thông báo là đã đọc.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
