using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
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
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Hồ sơ chuyên gia không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                expertProfile.Status = 1; // Approved
                expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Hồ sơ chuyên gia đã được duyệt thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình xét duyệt hồ sơ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}

