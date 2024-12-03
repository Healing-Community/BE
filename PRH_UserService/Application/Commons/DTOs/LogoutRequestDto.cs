using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.DTOs;

public class LogoutRequestDto
{
    public required string RefreshToken { get; set; }
}