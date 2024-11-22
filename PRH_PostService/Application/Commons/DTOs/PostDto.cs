namespace Application.Commons.DTOs
{
    public class PostDto
    {
        public string PostId { get; set; } = string.Empty; // Thêm PostId
        public string? UserId { get; set; } // Thêm UserId
        public string? GroupId { get; set; } // Thêm GroupId
        public string? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; } 
    }
}
