using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh token không được để trống")]
    public string? RefreshToken { get; set; }
}