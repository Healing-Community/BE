using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IJwtTokenRepository
{
    string GenerateToken(User user);
    string GenerateVerificationToken(User user);
    bool ValidateToken(string token, out string userId);
    string GenerateRefreshToken(User user);
    bool ValidateRefreshToken(string refreshToken, out string userId);
}