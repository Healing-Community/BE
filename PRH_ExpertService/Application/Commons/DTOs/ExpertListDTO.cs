namespace Application.Commons.DTOs
{
    public class ExpertListDTO
    {
        public required string Fullname { get; set; }
        public string? ProfileImageUrl { get; set; }
        public required string Specialization { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalRatings { get; set; }
    }
}
