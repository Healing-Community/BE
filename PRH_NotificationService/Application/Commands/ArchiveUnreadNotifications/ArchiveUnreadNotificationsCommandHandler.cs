using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.ArchiveUnreadNotifications
{
    public class ArchiveUnreadNotificationsCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<ArchiveUnreadNotificationsCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(ArchiveUnreadNotificationsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                await notificationRepository.ArchiveUnreadNotificationsAsync(request.UserId.ToString());

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
