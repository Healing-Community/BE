
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, BaseResponse<int>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<int>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var unreadCount = await _notificationRepository.GetUnreadCountAsync(request.UserId);
                response.Success = true;
                response.Data = unreadCount;
                response.StatusCode = 200;
                response.Message = unreadCount > 0 ? "Unread notifications found." : "No unread notifications.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while fetching unread notification count.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
