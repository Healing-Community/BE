using Application.Commands.ArchiveUnreadNotifications;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Notification
{
    public class ArchiveUnreadNotificationsCommandHandler : IRequestHandler<ArchiveUnreadNotificationsCommand, BaseResponse<string>>
    {
        private readonly INotificationRepository _notificationRepository;

        public ArchiveUnreadNotificationsCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<string>> Handle(ArchiveUnreadNotificationsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _notificationRepository.ArchiveUnreadNotificationsAsync(request.UserId);
                response.Success = true;
                response.Message = "Unread notifications archived successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to archive unread notifications.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
