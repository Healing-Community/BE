using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using AutoMapper;
using MediatR;
using NUlid;

namespace Application.Queries.GetExpertProfile
{
    public class GetExpertProfileQueryHandler(
        IExpertProfileRepository expertProfileRepository,
        IAppointmentRepository appointmentRepository,
        ICertificateRepository certificateRepository,
        IWorkExperienceRepository workExperienceRepository,
        IMapper mapper) : IRequestHandler<GetExpertProfileQuery, BaseResponse<ExpertProfileDTO>>
    {
        public async Task<BaseResponse<ExpertProfileDTO>> Handle(GetExpertProfileQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ExpertProfileDTO>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy ExpertProfile
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);

                // Map ExpertProfile
                var expertProfileDto = mapper.Map<ExpertProfileDTO>(expertProfile);

                // Lấy và ánh xạ Appointments
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);
                expertProfileDto.Appointments = mapper.Map<ICollection<AppointmentDTO>>(appointments);

                // Lấy và ánh xạ Certificates
                var certificates = await certificateRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);
                expertProfileDto.Certificates = mapper.Map<ICollection<CertificateDTO>>(certificates);

                // Lấy và ánh xạ WorkExperiences
                var workExperiences = await workExperienceRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);
                expertProfileDto.WorkExperiences = mapper.Map<ICollection<WorkExperienceDTO>>(workExperiences);

                response.Success = true;
                response.Data = expertProfileDto;
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