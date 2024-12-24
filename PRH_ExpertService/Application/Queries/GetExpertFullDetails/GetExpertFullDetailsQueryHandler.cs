using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetExpertFullDetails
{
    public class GetExpertFullDetailsQueryHandler(
        IExpertProfileRepository expertProfileRepository,
        IAppointmentRepository appointmentRepository,
        ICertificateRepository certificateRepository,
        IWorkExperienceRepository workExperienceRepository,
        IExpertAvailabilityRepository expertAvailabilityRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : IRequestHandler<GetExpertFullDetailsQuery, BaseResponse<ExpertFullDetailsDTO>>
    {
        public async Task<BaseResponse<ExpertFullDetailsDTO>> Handle(GetExpertFullDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ExpertFullDetailsDTO>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy ExpertProfileId từ token
                var expertProfileId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (string.IsNullOrEmpty(expertProfileId))
                {
                    response.Success = false;
                    response.Message = "Không thể xác thực người dùng.";
                    response.StatusCode = 401;
                    return response;
                }

                // Lấy ExpertProfile
                var expertProfile = await expertProfileRepository.GetByIdAsync(expertProfileId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy hồ sơ chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                // Map ExpertProfile
                var expertDetailsDto = mapper.Map<ExpertFullDetailsDTO>(expertProfile);

                // Lấy và ánh xạ WorkExperiences
                var workExperiences = await workExperienceRepository.GetByExpertProfileIdAsync(expertProfileId);
                expertDetailsDto.WorkExperiences = mapper.Map<ICollection<WorkExperienceDTO>>(workExperiences);

                // Lấy và ánh xạ Certificates
                var certificates = await certificateRepository.GetByExpertProfileIdAsync(expertProfileId);
                expertDetailsDto.Certificates = mapper.Map<ICollection<CertificateDTO>>(certificates);

                // Lấy và ánh xạ Availabilities
                var availabilities = await expertAvailabilityRepository.GetByExpertProfileIdAsync(expertProfileId);
                expertDetailsDto.Availabilities = mapper.Map<ICollection<ExpertAvailabilityDTO>>(availabilities);

                // Lấy và ánh xạ Appointments
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(expertProfileId);
                expertDetailsDto.Appointments = mapper.Map<ICollection<AppointmentDTO>>(appointments);

                response.Success = true;
                response.Data = expertDetailsDto;
                response.StatusCode = 200;
                response.Message = "Lấy thông tin chi tiết chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin chi tiết chuyên gia.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
