using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateVerificationToken(User user);
    bool ValidateRefreshToken(string refreshToken, out string userId);
    bool ValidateToken(string token, out string userId);
}