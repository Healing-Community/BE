namespace Application.Commons.DTOs
{
    public class ExpertRatingDTO
    {
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public decimal AverageRating { get; set; }
        public DateTime Time { get; set; }
    }
}
