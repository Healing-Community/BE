using System;

namespace Application.Commons.DTOs;

public class ShareDto
{
    public string? PostId { get; set; }
    public string? Description { get; set; }
    public string? Platform { get; set; } = "Internal";
}
