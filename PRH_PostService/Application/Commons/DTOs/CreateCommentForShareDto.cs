namespace Application.Commons.DTOs
{
    public class CreateCommentForShareDto
    {
        public required string ShareId { get; set; }
        public string? ParentId { get; set; } 
        public required string Content { get; set; }
        public string? CoverImgUrl { get; set; }
    }
}
