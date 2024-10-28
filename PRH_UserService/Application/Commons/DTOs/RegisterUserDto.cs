using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Tên người dùng không được để trống")]
    [MinLength(6, ErrorMessage = "Tên người dùng phải có ít nhất 6 ký tự")]
    [MaxLength(50, ErrorMessage = "Tên người dùng không được vượt quá 50 ký tự")]
    public string UserName { get; set; } = null!;
    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = null!;
    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;
}