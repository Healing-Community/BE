using Application.Commands.MarkNotificationAsRead;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Notification
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
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _notificationRepository.MarkAsReadAsync(request.NotificationId);
                response.Success = true;
                response.Message = "Notification marked as read successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to mark notification as read.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
