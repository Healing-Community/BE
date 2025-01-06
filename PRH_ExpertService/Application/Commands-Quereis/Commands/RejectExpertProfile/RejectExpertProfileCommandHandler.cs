using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.RejectExpertProfile
{
    public class RejectExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<RejectExpertProfileCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(RejectExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            try
            {
                // Kiểm tra xem hồ sơ chuyên gia có tồn tại không
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = $"Hồ sơ chuyên gia với ID '{request.ExpertProfileId}' không tồn tại.",
                        Field = "ExpertProfileId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
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
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Chi tiết lỗi: {ex.Message}",
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình từ chối hồ sơ.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
