using System;

namespace Application.Commons.DTOs;

public class ShareDto
{
    public string? PostId { get; set; }
    public string? Description { get; set; } = string.Empty;
    public string? Platform { get; set; } = "Internal";
}
