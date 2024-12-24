using System.Collections.Generic;

namespace Application.Commons.DTOs
{
    public class ExpertFullDetailsDTO
    {
        // Thông tin hồ sơ chuyên gia
        public required string ExpertProfileId { get; set; }
        public required string Fullname { get; set; }
        public required string Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        // Các thông tin khác của hồ sơ chuyên gia...

        // Danh sách kinh nghiệm làm việc
        public ICollection<WorkExperienceDTO> WorkExperiences { get; set; } = new List<WorkExperienceDTO>();

        // Danh sách chứng chỉ
        public ICollection<CertificateDTO> Certificates { get; set; } = new List<CertificateDTO>();

        // Danh sách lịch trống
        public ICollection<ExpertAvailabilityDTO> Availabilities { get; set; } = new List<ExpertAvailabilityDTO>();

        // Danh sách lịch hẹn
        public ICollection<AppointmentDTO> Appointments { get; set; } = new List<AppointmentDTO>();
    }
}
