using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.RejectExpertProfile
{
    public class RejectExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<RejectExpertProfileCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(RejectExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Kiểm tra xem hồ sơ chuyên gia có tồn tại không
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Errors.Add($"Hồ sơ chuyên gia với ID '{request.ExpertProfileId}' không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật trạng thái hồ sơ chuyên gia
                expertProfile.Status = 2; // Rejected
                expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);

                // Trả về kết quả thành công
                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Hồ sơ chuyên gia đã bị từ chối.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
