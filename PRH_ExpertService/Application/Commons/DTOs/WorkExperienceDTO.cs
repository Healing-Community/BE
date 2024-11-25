namespace Application.Commons.DTOs
{
    public class WorkExperienceDTO
    {
        public string WorkExperienceId { get; set; }
        public string CompanyName { get; set; }
        public string PositionTitle { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Description { get; set; }
    }
}