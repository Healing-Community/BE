using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token không được để trống")]
        public string? RefreshToken { get; set; }
    }
}
