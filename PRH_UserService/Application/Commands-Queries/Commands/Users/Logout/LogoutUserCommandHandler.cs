using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.Logout;

public class LogoutUserCommandHandler(ITokenRepository tokenRepository)
    : IRequestHandler<LogoutUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7),
            Errors = new List<string>() // Khởi tạo danh sách lỗi
        };

        try
        {
            // Kiểm tra xem request có dữ liệu hay không, nếu không thì throw lỗi
            if (request.LogoutRequestDto?.context == null)
            {
                response.StatusCode = 400;
                response.Success = false;
                response.Message = "Yêu cầu không hợp lệ.";
                response.Errors.Add("Dữ liệu context không tồn tại.");
                return response;
            }

            // Lấy userId từ HttpContext
            var userId =
                Authentication.GetUserIdFromHttpContext(request.LogoutRequestDto.context ??
                                                        throw new InvalidOperationException("Context không hợp lệ."));

            // Tìm token của người dùng trong kho dữ liệu
            var tokenUser = await tokenRepository.GetByPropertyAsync(t => t.UserId == userId);
            var refreshToken = request.LogoutRequestDto.RefreshToken;

            // Nếu token tìm được và refresh token hợp lệ
            if (tokenUser != null && tokenUser.RefreshToken == refreshToken)
            {
                await tokenRepository.DeleteAsync(tokenUser.TokenId); // Xoá token khỏi kho
                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Đăng xuất thành công.";
            }
            else
            {
                // Nếu token không hợp lệ hoặc không tìm thấy
                response.StatusCode = 401;
                response.Success = false;
                response.Message = "Đăng xuất không thành công.";
                response.Errors.Add("Refresh token không hợp lệ!");
            }
        }
        catch (InvalidOperationException ex)
        {
            // Bắt lỗi khi có exception InvalidOperationException xảy ra
            response.StatusCode = 400;
            response.Success = false;
            response.Message = "Yêu cầu không hợp lệ.";
            response.Errors.Add(ex.Message);
        }
        catch (Exception ex)
        {
            // Bắt lỗi tổng quát khác
            response.StatusCode = 500;
            response.Success = false;
            response.Message = "Đã xảy ra lỗi trong quá trình đăng xuất.";
            response.Errors.Add(ex.Message); // Thêm thông báo lỗi vào danh sách
        }

        return response;
    }
}