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
        try
        {
            var accessToken = request.TokenDto.Token;
            var refreshToken = request.TokenDto.RefreshToken;

            // Lấy thông tin từ token hết hạn
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken ?? "");
            var username = principal?.Identity?.Name;

            // Kiểm tra tính hợp lệ của token và refresh token
            if (string.IsNullOrEmpty(username) || refreshToken == null)
            {
                response.Success = false;
                response.Message = "Token không hợp lệ";
                response.StatusCode = 401;
                response.Errors = ["Token không hợp lệ"];
                return response;
            }

            // Tìm người dùng theo username
            var user = await userRepository.GetByPropertyAsync(u => u.UserName == username);
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
                // Tạo access token và refresh token mới
                var newAccessToken = tokenService.GenerateAccessToken(principal?.Claims ?? []);
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
        }catch(Exception e)
        {
            response.Success = false;
            response.Message = "Có lỗi xảy ra";
            response.StatusCode = 500;
            response.Errors = [e.Message];
        }
        return response;
    }
}
