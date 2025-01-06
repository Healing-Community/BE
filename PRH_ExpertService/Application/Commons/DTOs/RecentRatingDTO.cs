namespace Application.Commons.DTOs
{
    public class RecentRatingDTO
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
