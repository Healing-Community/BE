using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IJwtTokenRepository
{
    string GenerateToken(User user);
    string GenerateVerificationToken(User user);
    bool ValidateToken(string token, out Guid userId);
    string GenerateRefreshToken(User user);
    bool ValidateRefreshToken(string refreshToken, out Guid userId);
}