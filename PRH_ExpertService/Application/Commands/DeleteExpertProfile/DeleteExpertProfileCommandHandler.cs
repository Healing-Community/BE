using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.DeleteExpertProfile
{
    public class DeleteExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteExpertProfileCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(DeleteExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
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

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                await expertProfileRepository.DeleteAsync(request.ExpertProfileId);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Xóa hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xóa hồ sơ chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
