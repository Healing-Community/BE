using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.ApproveExpertProfile
{
    public class ApproveExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<ApproveExpertProfileCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(ApproveExpertProfileCommand request, CancellationToken cancellationToken)
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
                    response.Errors.Add($"Hồ sơ chuyên gia với ID '{request.ExpertProfileId}' không tồn tại.");
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật trạng thái hồ sơ chuyên gia
                expertProfile.Status = 1; // Approved
                expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);

                // Trả về kết quả thành công
                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Hồ sơ chuyên gia đã được duyệt thành công.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Success = false;
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
