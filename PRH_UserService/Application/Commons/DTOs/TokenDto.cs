using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class TokenDto
{
    [Required(ErrorMessage = "Token không được để trống")]
    public string? Token { get; set; }
    [Required(ErrorMessage = "Refresh token không được để trống")]
    public string? RefreshToken { get; set; }
}