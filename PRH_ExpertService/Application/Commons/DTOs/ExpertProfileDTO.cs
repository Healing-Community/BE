namespace Application.Commons.DTOs
{
    public class ExpertProfileDTO
    {
        public string ExpertProfileId { get; set; }
        public string UserId { get; set; }
        public string Specialization { get; set; }
        public string ExpertiseAreas { get; set; }
        public string Bio { get; set; }
        public string Fullname { get; set; }
        public string ProfileImageUrl { get; set; }
        public decimal AverageRating { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public ICollection<CertificateDTO> Certificates { get; set; }
        public ICollection<WorkExperienceDTO> WorkExperiences { get; set; }
        public ICollection<AppointmentDTO> Appointments { get; set; }
    }
}