using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetAvailability
{
    public class GetAvailabilityQueryHandler(IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetAvailabilityQuery, BaseResponse<IEnumerable<ExpertAvailability>>>
    {
        public async Task<BaseResponse<IEnumerable<ExpertAvailability>>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ExpertAvailability>>
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
                    response.Message = "Không thể xác định UserId từ yêu cầu.";
                    response.StatusCode = 401;
                    return response;
                }

                var availabilityList = await expertAvailabilityRepository.GetByExpertProfileIdAsync(userId);

                response.Success = true;
                response.Data = availabilityList;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch trống thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
