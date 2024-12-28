﻿namespace Application.Commons.DTOs
{
    public class PostGroupWithoutGroupIdDto
    {
        public string? PostId { get; set; }
        public string? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UserId { get; set; }
    }
}