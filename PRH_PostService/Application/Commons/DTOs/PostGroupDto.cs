﻿namespace Application.Commons.DTOs
{
    public class PostGroupDto
    {
        public string? CategoryId { get; set; }
        public string? GroupId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}