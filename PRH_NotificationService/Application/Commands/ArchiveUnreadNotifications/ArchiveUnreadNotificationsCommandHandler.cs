using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.ArchiveUnreadNotifications
{
    public class ArchiveUnreadNotificationsCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<ArchiveUnreadNotificationsCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(ArchiveUnreadNotificationsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                await notificationRepository.ArchiveUnreadNotificationsAsync(request.UserId);

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
