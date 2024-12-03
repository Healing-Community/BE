using System.Security.Claims;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.Logout;

public class LogoutUserCommandHandler(IHttpContextAccessor accessor,ITokenRepository tokenRepository)
    : IRequestHandler<LogoutUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy userId từ HttpContext
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);        
            if (userId == null)
            {
                return BaseResponse<string>.BadRequest("Không tìm thấy thông tin người dùng.");
            }
            // Tìm token của người dùng trong kho dữ liệu
            var tokenUser = await tokenRepository.GetByPropertyAsync(t => t.UserId == userId);
            var refreshToken = request.LogoutRequestDto.RefreshToken;

            // Nếu token tìm được và refresh token hợp lệ
            if (tokenUser != null && tokenUser.RefreshToken == refreshToken)
            {
                await tokenRepository.DeleteAsync(tokenUser.TokenId); // Xoá token khỏi kho
                return BaseResponse<string>.SuccessReturn("Đăng xuất thành công.");
            }
            else
            {
                // Nếu token không hợp lệ hoặc không tìm thấy
                return BaseResponse<string>.BadRequest("Token không hợp lệ.");
            }
        }
        catch (Exception ex)
        {
            // Bắt lỗi tổng quát khác
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}