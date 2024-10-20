using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using NUlid;

namespace Application.Commands.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, BaseResponse<TokenDto>>
{
    public async Task<BaseResponse<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<TokenDto>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };

        var accessToken = request.TokenDto.Token;
        var refreshToken = request.TokenDto.RefreshToken;

        var principal = tokenService.GetPrincipalFromExpiredToken(accessToken ?? "");
        var username = principal?.Identity?.Name;

        // Check user mail and expired token
        if (string.IsNullOrEmpty(username) || refreshToken == null)
        {
            response.Success = false;
            response.Message = "Invalid token";
            response.StatusCode = 401;
            return response;
        }


        var user = await userRepository.GetByPropertyAsync(u => u.UserName == username);
        var token = await tokenRepository.GetByPropertyAsync(u => u.UserId == user.UserId);
        if (user == null || token.RefreshToken != refreshToken)
        {
            response.Success = false;
            response.Message = "Invalid token";
            response.StatusCode = 401;
            response.Data = null;
        }
        else
        {
            var newAccessToken = tokenService.GenerateAccessToken(principal?.Claims ?? []);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            response.Success = true;
            response.Message = "Token refreshed successfully";
            // Logic to update the refress token in the database
            token.RefreshToken = newRefreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60"));
            await tokenRepository.Update(token.UserId, token);

            // Set response details
            response.Success = true;
            response.Message = "Token refreshed successfully";
            response.StatusCode = 200;
            response.Data = new TokenDto
            {
                RefreshToken = newRefreshToken,
                Token = newAccessToken
            };
        }

        return response;
    }
}