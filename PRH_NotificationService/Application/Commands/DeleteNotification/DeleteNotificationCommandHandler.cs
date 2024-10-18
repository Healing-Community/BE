using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<DeleteNotificationCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                await notificationRepository.DeleteAsync(request.NotificationId);
                response.Success = true;
                response.Data = "Xóa thông báo thành công";
                response.StatusCode = 200;
                response.Message = "Thông báo đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi xóa thông báo.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}

