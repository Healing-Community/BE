using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetReadNotificationRate
{
    public class GetReadNotificationRateQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetReadNotificationRateQuery, BaseResponse<double>>
    {
        public async Task<BaseResponse<double>> Handle(GetReadNotificationRateQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<double>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var readRate = await notificationRepository.GetReadNotificationRateAsync();
                response.Data = readRate;
                response.Success = true;
                response.Message = "Tỷ lệ thông báo đã đọc đã được lấy thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lấy tỷ lệ thông báo đã đọc. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
