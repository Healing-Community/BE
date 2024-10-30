using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetExpertProfile
{
    public class GetExpertProfileQueryHandler(
        IExpertProfileRepository expertProfileRepository,
        ICertificateRepository certificateRepository,
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<GetExpertProfileQuery, BaseResponse<ExpertProfile>>
    {
        public async Task<BaseResponse<ExpertProfile>> Handle(GetExpertProfileQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ExpertProfile>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                expertProfile.Certificates = (await certificateRepository.GetCertificatesByExpertIdAsync(request.ExpertProfileId)).ToList();
                expertProfile.WorkExperiences = (await workExperienceRepository.GetWorkExperiencesByExpertIdAsync(request.ExpertProfileId)).ToList();

                response.Success = true;
                response.Data = expertProfile;
                response.StatusCode = 200;
                response.Message = "Lấy thông tin hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy hồ sơ chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
