using Application.Commands_Queries.Commands.Users.RefreshToken;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using NUlid;
using System.Security.Claims;

namespace Application.Commands.Users.RefreshToken;

public class RefreshTokenCommandHandler(
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService, IRoleRepository roleRepository) : IRequestHandler<RefreshTokenCommand, BaseResponse<TokenDto>>
{
    public async Task<BaseResponse<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<TokenDto>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        try
        {
            var refreshToken = request.RefreshTokenDto.RefreshToken;

            // Lấy thông tin từ token hết hạn
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

            // Kiểm tra tính hợp lệ của token và refresh token
            if (string.IsNullOrEmpty(userId) || refreshToken == null)
            {
                response.Success = false;
                response.Message = "Token không hợp lệ";
                response.StatusCode = 401;
                response.Errors = ["Token không hợp lệ"];
                return response;
            }

            // Tìm người dùng theo username
            var user = await userRepository.GetByPropertyAsync(u => u.UserId == userId);
            var token = await tokenRepository.GetByPropertyAsync(u => u.UserId == user.UserId);

            // Kiểm tra token của người dùng và refresh token có khớp không
            if (user == null || token == null || token.RefreshToken != refreshToken)
            {
                response.Success = false;
                response.Message = "Token không hợp lệ";
                response.StatusCode = 401;
                response.Data = null;
                response.Errors = ["Token không hợp lệ"];

            }
            else
            {
                var roleName = roleRepository.GetRoleNameById(user.RoleId);
                var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, roleName.Result ?? "None")
            };
                // Tạo access token và refresh token mới
                var newAccessToken = tokenService.GenerateAccessToken(claims ?? []);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                // Cập nhật refresh token trong cơ sở dữ liệu
                token.RefreshToken = newRefreshToken;
                token.ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60"));
                await tokenRepository.Update(token.TokenId, token);

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
}
