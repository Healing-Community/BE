namespace Application.Commons.DTOs
{
    public class PostDetailDto
    {
        public string PostId { get; set; } = string.Empty; 
        public string? UserId { get; set; } 
        public string? GroupId { get; set; } 
        public string? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
