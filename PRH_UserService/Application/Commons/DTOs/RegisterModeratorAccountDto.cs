using System;

namespace Application.Commons.DTOs;

public class RegisterModeratorAccountDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } =  string.Empty;
    public string PhoneNumber { get; set; } =  string.Empty;
}
