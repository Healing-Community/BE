using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<DeleteNotificationCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                await notificationRepository.DeleteAsync(request.NotificationId);
                response.Success = true;
                response.Message = "Thông báo đã được xóa thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi xóa thông báo. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}

