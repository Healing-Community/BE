namespace Application.Commons.DTOs
{
    public class PostDetailShareDto
    {
        public required string ShareId { get; set; } 
        public required string PostId { get; set; }
        public required string? UserId { get; set; } 
        public required string? GroupId { get; set; } 
        public required string? CategoryId { get; set; }
        public string ShareDescription { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? CoverImgUrl { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime? ShareAt { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
