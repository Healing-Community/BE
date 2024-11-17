using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class SendForgotPasswordDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = null!;
}