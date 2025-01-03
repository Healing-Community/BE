﻿namespace Domain.Entities
{
    public class Post
    {
        public required string PostId { get; init; }
        public string? UserId { get; set; }
        public string? GroupId { get; set; }
        public string? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Comment>? Comments { get; set; } 
        public ICollection<Reaction>? Reactions { get; set; }
        public ICollection<Report>? Reports { get; set; }
        public ICollection<BookmarkPost>? BookmarkPosts { get; set; }
        public ICollection<Share>? Shares { get; set; }
        public Category? Category { get; set; }
    }
}
