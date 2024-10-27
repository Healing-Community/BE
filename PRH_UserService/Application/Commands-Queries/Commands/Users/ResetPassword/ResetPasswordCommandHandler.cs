using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Users.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, ITokenRepository tokenRepository) : IRequestHandler<ResetPasswordCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = []
        };

        try
        {
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
            if (string.IsNullOrEmpty(userId))
            {
                
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Không tìm thấy thông tin người dùng.";
                response.Success = false;
                return response;
            }
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Không tìm thấy thông tin người dùng.";
                response.Success = false;
                return response;
            }


            if (!BCrypt.Net.BCrypt.Verify(request.ResetPasswordDto.OldPassword, user.PasswordHash))
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu cũ không chính xác.",
                    Field = "old-password"
                });
            }

            // Bắt lỗi cho mật khẩu
            if (request.ResetPasswordDto.NewPassword.Length < 8)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu phải có ít nhất 8 ký tự.",
                    Field = "password"
                });
            }

            if (request.ResetPasswordDto.NewPassword != request.ResetPasswordDto.ConfirmPassword)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu mới và xác nhận mật khẩu không khớp.",
                    Field = "re-password"
                });
            }

            if (response.Errors.Count != 0)
            {
                response.Message = "Dữ liệu không hợp lệ.";
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Success = false;
                return response;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
            // Update user password 
            await userRepository.Update(user.UserId,user);
            // Delete user token
            var token = await tokenRepository.GetByPropertyAsync(t => t.UserId == userId);
            await tokenRepository.DeleteAsync(token.TokenId);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Message = "Cập nhật mật khẩu thành công.";
            response.Success = true;
            return response;

        }
        catch (Exception ex)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Message = "Có lỗi xảy ra khi thực hiện yêu cầu.";
            response.Success = false;
            response.Errors.Add(new ErrorDetail
            {
                Message = ex.Message,
                Field = "exception"
            });
        }
        return response;
    }
}