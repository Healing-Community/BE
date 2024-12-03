using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
namespace Application.Commands_Queries.Commands.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService,
    IRoleRepository roleRepository) : IRequestHandler<RefreshTokenCommand, BaseResponse<TokenDto>>
{
    public async Task<BaseResponse<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kiểm tra refresh token có hết hạn không
            if (tokenService.IsRefreshTokenExpired(request.RefreshTokenDto.RefreshToken))
            {
                return BaseResponse<TokenDto>.CustomResponse(401, "Refresh token đã hết hạn", false, ["Refresh token đã hết hạn"]);
            }

            // Tìm người dùng theo username
            var token = await tokenRepository.GetByPropertyAsync(u =>
                u.RefreshToken == request.RefreshTokenDto.RefreshToken);
            if (token == null || token.RefreshToken != request.RefreshTokenDto.RefreshToken)
            {
                return BaseResponse<TokenDto>.CustomResponse(401, "Token không hợp lệ", false, ["Token không hợp lệ"]);
            }

            // Lấy thông tin người dùng từ token đã tìm được để tạo mới token 
            var user = await userRepository.GetByPropertyAsync(u => u.UserId == token.UserId);
            if (user == null)
            {
                return BaseResponse<TokenDto>.CustomResponse(404, "Người dùng không tồn tại", false, ["Người dùng không tồn tại"]);
            }

            var role = await roleRepository.GetByPropertyAsync(r => r.RoleId == user.RoleId) ?? new Role();
            var accessTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UserId),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role.RoleName ?? "Unknown")
            };
            // Tạo claims cho refresh token
            var refreshTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };
            // Tạo access token và refresh token mới
            var newAccessToken = tokenService.GenerateAccessToken(accessTokenClaims);
            // Tạo refresh token mới
            var newRefreshToken = tokenService.GenerateRefreshToken(token.RefreshToken, refreshTokenClaims);

            // Cập nhật refresh token trong cơ sở dữ liệu
            bool isTokenUpdate = await UpdateRefreshToken(token, newRefreshToken);
            // Thiết lập chi tiết phản hồi
            if (!isTokenUpdate)
            {
                return BaseResponse<TokenDto>.CustomResponse(500, "Có lỗi xảy ra", false, ["Có lỗi xảy ra"]);
            }
            else
            {
                var tokenDto = new TokenDto
                {
                    RefreshToken = newRefreshToken,
                    Token = newAccessToken
                };
                return BaseResponse<TokenDto>.SuccessReturn(tokenDto, "Tạo token thành công");
            }
        }
        catch (Exception e)
        {
            return BaseResponse<TokenDto>.InternalServerError(message: e.Message);
        }
    }

    private async Task<bool> UpdateRefreshToken(Token token, string newRefreshToken)
    {
        try
        {
            token.RefreshToken = newRefreshToken;
            token.IssuedAt = DateTime.UtcNow.AddHours(7);
            token.ExpiresAt =
                DateTime.UtcNow.AddMinutes(60 * 7 + int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? ""));
            await tokenRepository.UpdateAsync(token.TokenId, token);
        }
        catch
        {
            return false;
            throw;
        }
        return true;
    }
}