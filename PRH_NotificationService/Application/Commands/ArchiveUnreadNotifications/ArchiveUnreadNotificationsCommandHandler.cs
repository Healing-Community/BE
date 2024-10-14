using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchiveUnreadNotifications
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
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                await _notificationRepository.ArchiveUnreadNotificationsAsync(request.UserId);

                response.Success = true;
                response.Message = "Thông báo chưa đọc đã được lưu trữ.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lưu trữ thông báo chưa đọc.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
