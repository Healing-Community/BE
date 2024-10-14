using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.DTOs;

public class LogoutRequestDto
{
    [JsonIgnore] public HttpContext? context { get; set; }

    public required string RefreshToken { get; set; }
}