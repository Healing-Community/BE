using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.LoginUser;

public class LoginUserCommandHandler(
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IRoleRepository roleRepository,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<LoginUserCommand, BaseResponse<TokenDto>>
{
    public async Task<BaseResponse<TokenDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Tìm kiếm người dùng bằng email
            var user = await userRepository.GetUserByEmailAsync(request.LoginDto.Email);
            if (user == null)
            {
                return BaseResponse<TokenDto>.CustomResponse(StatusCodes.Status422UnprocessableEntity,"Email hoặc mật khẩu không đúng.",false,["Email hoặc mật khẩu không đúng."]);
            }
            // Kiểm tra trạng thái người dùng
            if (user.Status == 0)
            {
                return BaseResponse<TokenDto>.CustomResponse(StatusCodes.Status401Unauthorized,"Tài khoản của bạn đã bị khóa.",false,["Tài khoản của bạn đã bị khóa."]);
            }

            // Xác thực mật khẩu
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                // Nếu mật khẩu không khớp
                if (request.LoginDto.Password == user.PasswordHash)
                {
                    // Nếu mật khẩu cần được mã hóa lại
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.LoginDto.Password);
                    await userRepository.UpdateAsync(user.UserId, user);
                    isPasswordValid = true;
                }
                else
                {
                    return BaseResponse<TokenDto>.CustomResponse(StatusCodes.Status422UnprocessableEntity,"Email hoặc mật khẩu không đúng.",false,["Email hoặc mật khẩu không đúng."]);
                }
            }

            // Lấy vai trò của người dùng
            var role = await roleRepository.GetByPropertyAsync(r => r.RoleId == user.RoleId);
            var accessTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UserId),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role?.RoleName ?? "Unknown")
            };

            // Tạo Access token
            var accessToken = tokenService.GenerateAccessToken(accessTokenClaims);

            var refreshTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var tokenData = new TokenDto
            {
                Token = accessToken,
                RefreshToken = tokenService.GenerateRefreshToken(string.Empty, refreshTokenClaims)
            };

            // Lưu Refresh token vào cơ sở dữ liệu
            var userToken = await tokenRepository.GetByPropertyAsync(t => t.UserId == user.UserId);

            if (userToken != null) await tokenRepository.DeleteAsync(userToken.TokenId);

            var token = new Token
            {
                TokenId = Ulid.NewUlid().ToString(),
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow.AddHours(7),
                RefreshToken = tokenData.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60 * 7 +
                                                       int.Parse(configuration["JwtSettings:RefreshTokenExpiryMinutes"] ?? "60"))
            };
            await tokenRepository.Create(token);
            
            return BaseResponse<TokenDto>.SuccessReturn(tokenData, "Đăng nhập thành công");
        }
        catch (Exception ex)
        {
            return BaseResponse<TokenDto>.InternalServerError(ex.Message);
        }
    }
}