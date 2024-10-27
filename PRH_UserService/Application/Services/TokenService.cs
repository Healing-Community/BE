using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public bool ValidateRefreshToken(string refreshToken, out string userId)
    {
        userId = Ulid.NewUlid().ToString();
        var jwtSettings = configuration.GetSection("JwtSettings");
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
        catch
        {
            return false;
        }
    }
    public (string?, bool) ValidateToken(string token)
    {
        var userId = Ulid.NewUlid().ToString();
        // Đặt userId thành null ban đầu để chỉ định không có giá trị
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
            }, out SecurityToken validatedToken);
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

    public string GenerateVerificationToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"] ??
                                                                  throw new InvalidOperationException(
                                                                      "Secret key is not configured.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("verification", "true")
        };

        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(5), // 1 minute for verification
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}