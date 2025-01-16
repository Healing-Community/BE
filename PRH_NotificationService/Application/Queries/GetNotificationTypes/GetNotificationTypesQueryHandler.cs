using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetNotificationTypes
{
    public class GetNotificationTypesQueryHandler(INotificationTypeRepository notificationTypeRepository) : IRequestHandler<GetNotificationTypesQuery, BaseResponse<List<NotificationType>>>
    {
        public async Task<BaseResponse<List<NotificationType>>> Handle(GetNotificationTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<NotificationType>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var notificationTypes = await notificationTypeRepository.GetsAsync();

                response.Success = true;
                response.Data = notificationTypes.ToList();
                response.StatusCode = 200;
                response.Message = "Lấy loại thông báo thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy loại thông báo. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
