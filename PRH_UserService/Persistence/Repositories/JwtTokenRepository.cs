using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class JwtTokenRepository : IJwtTokenRepository
    {
        private readonly IConfiguration _configuration;

        public JwtTokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("Secret key is not configured.");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Issuer is not configured.");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Audience is not configured.");
            var expiryMinutesStr = jwtSettings["ExpiryMinutes"] ?? throw new InvalidOperationException("Expiry minutes are not configured.");

            if (!int.TryParse(expiryMinutesStr, out int expiryMinutes))
            {
                throw new InvalidOperationException("Expiry minutes must be a valid integer.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
