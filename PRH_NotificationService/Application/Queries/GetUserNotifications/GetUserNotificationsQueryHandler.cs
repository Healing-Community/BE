using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetUserNotificationsQuery, BaseResponse<List<NotificationDto>>>
    {
        public async Task<BaseResponse<List<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<NotificationDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var notifications = await notificationRepository.GetNotificationsByUserAsync(request.UserId, request.IncludeRead);

                response.Success = true;
                response.Data = notifications.Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    NotificationTypeId = n.NotificationTypeId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList();
                response.StatusCode = 200;
                response.Message = "Lấy thông báo thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông báo.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
