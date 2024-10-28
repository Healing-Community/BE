using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu cũ phải có ít nhất 6 ký tự")]
    public string OldPassword { get; set; } = null!;
    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = null!;
    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;
}