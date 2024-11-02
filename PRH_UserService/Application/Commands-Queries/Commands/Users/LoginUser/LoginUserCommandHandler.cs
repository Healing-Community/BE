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
        var response = new BaseResponse<TokenDto>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7)
        };
        try
        {
            // Tìm kiếm người dùng bằng email
            var user = await userRepository.GetUserByEmailAsync(request.LoginDto.Email);
            if (user == null)
                return new BaseResponse<TokenDto>
                {
                    Id = Ulid.NewUlid().ToString(),
                    Success = false,
                    Message = "Email hoặc mật khẩu không đúng.",
                    Errors = ["Email hoặc mật khẩu không đúng."],
                    Timestamp = DateTime.UtcNow.AddHours(7),

                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

            // Kiểm tra trạng thái người dùng
            if (user.Status == 0)
                return new BaseResponse<TokenDto>
                {
                    Id = Ulid.NewUlid().ToString(),
                    Success = false,
                    Message = "Tài khoản người dùng đã bị vô hiệu hóa.",
                    Errors = ["Tài khoản đã bị vô hiệu hóa."],
                    Timestamp = DateTime.UtcNow.AddHours(7),
                    StatusCode = StatusCodes.Status401Unauthorized
                };

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
                    return new BaseResponse<TokenDto>
                    {
                        Id = Ulid.NewUlid().ToString(),
                        Success = false,
                        Message = "Email hoặc mật khẩu không đúng.",
                        Errors = ["Email hoặc mật khẩu không đúng."],
                        Timestamp = DateTimeOffset.UtcNow,
                        StatusCode = StatusCodes.Status422UnprocessableEntity
                    };
                }
            }

            // Lấy vai trò của người dùng
            var roleName = roleRepository.GetRoleNameById(user.RoleId);
            var accessTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UserId),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, roleName.Result ?? "None")
            };

            // Tạo Access token
            var accessToken = tokenService.GenerateAccessToken(accessTokenClaims);
            response.Success = true;
            response.Message = "Đăng nhập thành công.";

            var refreshTokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var tokenData = new TokenDto
            {
                Token = accessToken,
                RefreshToken = tokenService.GenerateRefreshToken(refreshTokenClaims)
            };

            response.Data = tokenData;

            // Lưu Refresh token vào cơ sở dữ liệu
            var userToken = await tokenRepository.GetByPropertyAsync(t => t.UserId == user.UserId);

            if (userToken != null) await tokenRepository.DeleteAsync(userToken.TokenId);

            var token = new Token
            {
                TokenId = Ulid.NewUlid().ToString(),
                UserId = user.UserId,
                RefreshToken = tokenData.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60 * 7 +
                                                       int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60"))
            };
            await tokenRepository.Create(token);
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "Đăng nhập không thành công.";
            response.Errors = [ex.StackTrace];
        }

        return response;
    }
}