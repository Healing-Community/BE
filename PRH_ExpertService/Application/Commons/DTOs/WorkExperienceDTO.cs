namespace Application.Commons.DTOs
{
    public class WorkExperienceDTO
    {
        public string WorkExperienceId { get; set; }
        public string CompanyName { get; set; }
        public string PositionTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}