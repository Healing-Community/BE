using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NUlid;

namespace Persistence.Repositories;

public class JwtTokenRepository : IJwtTokenRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenRepository> _logger;

    public JwtTokenRepository(IConfiguration configuration, ILogger<JwtTokenRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(User user)
    {
        var (secretKey, issuer, audience, expiryMinutes) = GetJwtSettings("ExpiryMinutes");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public string GenerateRefreshToken(User user)
    {
        var (secretKey, issuer, audience, expiryDays) = GetJwtSettings("RefreshTokenExpiryDays");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateRefreshToken(string refreshToken, out string userId)
    {
        userId = Ulid.NewUlid().ToString();
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Secret key is not configured.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh token validation failed.");
            return false;
        }
    }

    public bool ValidateToken(string token, out string userId)
    {
        userId = Ulid.NewUlid().ToString();
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Secret key is not configured.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            if (userIdClaim != null)
            {
                userId = userIdClaim.Value;
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed.");
        }

        return false;
    }

    public string GenerateVerificationToken(User user)
    {
        var (secretKey, issuer, audience, expiryMinutes) = GetJwtSettings("VerificationExpiryMinutes");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("verification", "true")
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private (string SecretKey, string Issuer, string Audience, int ExpiryMinutes) GetJwtSettings(string expiryKey)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Secret key is not configured.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Issuer is not configured.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Audience is not configured.");
        var expiryMinutesStr = jwtSettings[expiryKey] ??
                               throw new InvalidOperationException($"{expiryKey} is not configured.");

        if (!int.TryParse(expiryMinutesStr, out var expiryMinutes))
            throw new InvalidOperationException($"{expiryKey} must be a valid integer.");

        return (secretKey, issuer, audience, expiryMinutes);
    }
}