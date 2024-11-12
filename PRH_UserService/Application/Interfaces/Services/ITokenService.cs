using System.Runtime.CompilerServices;
using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(string oldRefreshToken,IEnumerable<Claim> claims);
    string GenerateVerificationToken(User user);
    bool IsRefreshTokenExpired(string? refreshToken);
    (string?, bool) ValidateToken(string token);
}