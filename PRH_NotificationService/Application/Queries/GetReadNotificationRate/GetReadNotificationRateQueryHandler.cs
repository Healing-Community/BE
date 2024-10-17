using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.GetReadNotificationRate
{
    public class GetReadNotificationRateQueryHandler : IRequestHandler<GetReadNotificationRateQuery, BaseResponse<double>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetReadNotificationRateQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<BaseResponse<double>> Handle(GetReadNotificationRateQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<double>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var readRate = await _notificationRepository.GetReadNotificationRateAsync();
                response.Data = readRate;
                response.Success = true;
                response.Message = "Tỷ lệ thông báo đã đọc đã được lấy thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lấy tỷ lệ thông báo đã đọc.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
