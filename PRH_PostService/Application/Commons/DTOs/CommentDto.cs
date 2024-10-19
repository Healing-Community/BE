namespace Application.Commons.DTOs
{
    public class CommentDto
    {
        public Guid PostId { get; set; }
        //public Guid? ParentId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
