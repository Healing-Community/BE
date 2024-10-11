using Domain.Entities;
using System;
using System.Security.Claims;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateVerificationToken(User user);
    bool ValidateRefreshToken(string refreshToken, out Guid userId);
    bool ValidateToken(string token, out Guid userId);
}
