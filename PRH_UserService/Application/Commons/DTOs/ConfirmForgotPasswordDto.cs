using System;

namespace Application.Commons.DTOs;

public class ConfirmForgotPasswordDto
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
