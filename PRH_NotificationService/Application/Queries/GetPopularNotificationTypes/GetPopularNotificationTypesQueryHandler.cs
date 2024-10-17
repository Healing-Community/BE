using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.GetPopularNotificationTypes
{
    public class GetPopularNotificationTypesQueryHandler : IRequestHandler<GetPopularNotificationTypesQuery, BaseResponse<Dictionary<string, int>>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetPopularNotificationTypesQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<Dictionary<string, int>>> Handle(GetPopularNotificationTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Dictionary<string, int>>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var popularTypes = await _notificationRepository.GetPopularNotificationTypesAsync();
                response.Data = popularTypes;
                response.Success = true;
                response.Message = "Các loại thông báo phổ biến đã được lấy thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lấy các loại thông báo phổ biến.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
