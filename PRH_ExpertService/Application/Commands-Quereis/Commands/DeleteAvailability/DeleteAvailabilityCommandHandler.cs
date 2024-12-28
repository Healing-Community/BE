using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.DeleteAvailability
{
    public class DeleteAvailabilityCommandHandler(
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteAvailabilityCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
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

                var availability = await expertAvailabilityRepository.GetByIdAsync(request.expertAvailabilityId);
                if (availability == null)
                {
                    response.Success = false;
                    response.Message = "Lịch trống không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                await expertAvailabilityRepository.DeleteAsync(request.expertAvailabilityId);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Xóa lịch trống thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xóa lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
