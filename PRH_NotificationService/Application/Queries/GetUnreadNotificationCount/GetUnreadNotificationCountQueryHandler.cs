using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetUnreadNotificationCount
{
    public class GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUnreadNotificationCountQuery, BaseResponse<int>>
    {
        public async Task<BaseResponse<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<int>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                var unreadCount = await notificationRepository.GetUnreadCountAsync(userId);
                response.Success = true;
                response.Data = unreadCount;
                response.StatusCode = 200;
                response.Message = unreadCount > 0 ? "Có thông báo chưa đọc." : "Không có thông báo chưa đọc.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy số lượng thông báo chưa đọc. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
