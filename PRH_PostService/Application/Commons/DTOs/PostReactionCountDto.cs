using System;

namespace Application.Commons.DTOs;

public class PostReactionCountDto
{
    public string? PostId { get; set; }
    public int Like { get; set; }
    public int Love { get; set; }
    public int Haha { get; set; }
    public int Wow { get; set; }
    public int Sad { get; set; }
    public int Angry { get; set; }
    public int Total { get; set; }
}
