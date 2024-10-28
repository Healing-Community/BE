using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class ConfirmForgotPasswordDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Mã OTP không được để trống")]
    [StringLength(6, ErrorMessage = "Mã OTP phải có 6 ký tự", MinimumLength = 6)]
    public string Otp { get; set; } = null!;
    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = null!;
    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;
}
