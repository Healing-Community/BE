using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Commons.DTOs
{
    public class LogoutRequestDto
    {
        [JsonIgnore]
        public HttpContext? context { get; set; }
        public required string RefreshToken { get; set; }
    }
}
