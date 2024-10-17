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
                response.Message = unreadCount > 0 ? "Có thông báo chưa đọc." : "Không có thông báo chưa đọc.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy số lượng thông báo chưa đọc.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
