using System;

namespace Application.Commons.DTOs;

public class FollowingUserDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? ProfilePicture { get; set; }
}
