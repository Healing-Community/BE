namespace Application.Commons.DTOs
{
    public class ExpertRatingsResponseDTO
    {
        public decimal AverageRating { get; set; }
        public IEnumerable<ExpertRatingDTO> Ratings { get; set; }
    }
}

