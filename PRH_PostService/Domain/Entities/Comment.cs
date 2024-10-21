﻿namespace Domain.Entities
{
    public class Comment
    {
        public required string CommentId { get; set; }
        public string? PostId { get; set; }
        public string? ParentId { get; set; }
        public string? UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Post Post { get; set; } = null!;
        public Comment Parent { get; set; } 
        public ICollection<Comment> Replies { get; set; } 
    }

}
