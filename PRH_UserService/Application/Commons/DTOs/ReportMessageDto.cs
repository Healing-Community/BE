﻿using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.DTOs;

public class ReportMessageDto
{
    [JsonIgnore] public HttpContext? context { get; set; }
    public string? TargetUserId { get; set; }
    public string? ExpertId { get; set; }
    public string? PostId { get; set; }
    public string? CommentId { get; set; }
    public string? Description { get; set; }

    public required string ReportTypeId { get; set; }
}