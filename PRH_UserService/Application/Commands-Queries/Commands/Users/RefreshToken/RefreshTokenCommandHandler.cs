﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using NUlid;

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
        var response = new BaseResponse<TokenDto>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7)
        };
        try
        {
            // Kiểm tra refresh token có hết hạn không
            if (tokenService.IsRefreshTokenExpired(request.RefreshTokenDto.RefreshToken))
            {
                response.Success = false;
                response.Message = "Refresh token đã hết hạn";
                response.StatusCode = 401;
                response.Data = null;
                response.Errors = ["Refresh token đã hết hạn"];
                return response;
            }

            // Tìm người dùng theo username
            var token = await tokenRepository.GetByPropertyAsync(u =>
                u.RefreshToken == request.RefreshTokenDto.RefreshToken);
            if (token == null || token.RefreshToken != request.RefreshTokenDto.RefreshToken)
            {
                response.Success = false;
                response.Message = "Token không hợp lệ";
                response.StatusCode = 401;
                response.Data = null;
                response.Errors = ["Token không hợp lệ"];
                return response;
            }

            // Lấy thông tin người dùng từ token đã tìm được để tạo mới token 
            var user = await userRepository.GetByPropertyAsync(u => u.UserId == token.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Người dùng không tồn tại";
                response.StatusCode = 404;
                response.Data = null;
                response.Errors = ["Người dùng không tồn tại"];
                return response;
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
            var newRefreshToken = tokenService.GenerateRefreshToken(token.RefreshToken,refreshTokenClaims);

            // Cập nhật refresh token trong cơ sở dữ liệu
            UpdateRefreshToken(token, newRefreshToken).Wait();
            // Thiết lập chi tiết phản hồi
            response.Success = true;
            response.Message = "Tạo mới token thành công";
            response.StatusCode = 200;
            response.Data = new TokenDto
            {
                RefreshToken = newRefreshToken,
                Token = newAccessToken
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = "Có lỗi xảy ra";
            response.StatusCode = 500;
            response.Errors = [e.Message];
        }

        return response;
    }

    private async Task UpdateRefreshToken(Token token, string newRefreshToken)
    {
        token.RefreshToken = newRefreshToken;
        token.IssuedAt = DateTime.UtcNow.AddHours(7);
        token.ExpiresAt =
            DateTime.UtcNow.AddMinutes(60 * 7 + int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? ""));
        await tokenRepository.UpdateAsync(token.TokenId, token);
    }
}