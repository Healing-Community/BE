using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    string GenerateVerificationToken(User user);
    bool ValidateRefreshToken(string refreshToken, out string userId);
    (string?, bool) ValidateToken(string token);

}