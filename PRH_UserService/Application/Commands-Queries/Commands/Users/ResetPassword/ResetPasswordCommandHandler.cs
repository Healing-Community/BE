using System.Net;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, ITokenRepository tokenRepository)
    : IRequestHandler<ResetPasswordCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7),
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
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu cũ không chính xác.",
                    Field = "old-password"
                });
            if (response.Errors.Count != 0)
            {
                response.Message = "Dữ liệu không hợp lệ.";
                response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response.Success = false;
                return response;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
            // Update user password 
            await userRepository.UpdateAsync(user.UserId, user);
            // Delete user token
            var token = await tokenRepository.GetByPropertyAsync(t => t.UserId == userId);
            if (token != null)
            {
                await tokenRepository.DeleteAsync(token.TokenId);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Cập nhật mật khẩu thành công.";
                response.Success = true;
                return response;
            }
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