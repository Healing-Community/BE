using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NUlid;

namespace Application.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ?? ""));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
            configuration["JwtSettings:Issuer"],
            configuration["JwtSettings:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60")),
            signingCredentials: signInCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public string GenerateRefreshToken(string oldRefreshToken, IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ?? ""));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        // Set default expiration time
        var exp = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:RefreshTokenExpiryMinutes"] ?? "10080"));

        // Step 1: If oldRefreshToken is provided, retrieve the `exp` claim and use it as the expiration time for the new token
        if (!string.IsNullOrEmpty(oldRefreshToken))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(oldRefreshToken) as JwtSecurityToken;

            // Extract the `exp` claim if it exists
            var expiresAt = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expiresAt != null && long.TryParse(expiresAt, out var expUnix))
            {
                // Convert the `exp` claim from Unix timestamp to DateTime
                exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }
        }

        var tokeOptions = new JwtSecurityToken(
            configuration["JwtSettings:Issuer"],
            configuration["JwtSettings:Audience"],
            claims,
            expires: exp,  // Use the old token's expiration time
            signingCredentials: signInCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }



    public bool IsRefreshTokenExpired(string? refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ?? "");
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true, // This will check the expiration
            ClockSkew = TimeSpan.Zero // Eliminates default 5-minute buffer
        };

        try
        {
            // Validate token; throws if invalid or expired
            tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
            return false; // Token is valid and not expired
        }
        catch (SecurityTokenExpiredException)
        {
            return true; // Token is expired
        }
        catch
        {
            return false; // Token is invalid for other reasons
        }
    }

    // this method is used to validate the token sent by the user
    public (string?, bool) ValidateToken(string token)
    {
        var userId = Ulid.NewUlid().ToString();
        var jwtSettings = configuration.GetSection("JwtSettings");
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
                ValidateLifetime = true, // Kiểm tra thời gian sống của token
                ClockSkew = TimeSpan.Zero // Không có sai số thời gian
            }, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim != null)
            {
                //xác thực thành công
                userId = userIdClaim.Value;
                return (userId, true);
            }
        }
        catch
        {
            //xác thực thất bại
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var subClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            return (subClaim?.Value, false);
        }

        return default;
    }

    // GenerateVerificationToken method
    public string GenerateVerificationToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"] ??
                                                                  throw new InvalidOperationException(
                                                                      "Secret key is not configured.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("verification", "true")
        };

        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["VerificationTokenExpiryMinutes"] ?? "60")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}