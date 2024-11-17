using System.Net;
using Application.Commons;
using Application.Interfaces.Redis;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.ForgotPassword.ConfirmForgotPassword;

public class ConfirmForgotPasswordCommandHandler(IUserRepository userRepository, IOtpCache otpCache)
    : IRequestHandler<ConfirmForgotPasswordCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(ConfirmForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = []
        };
        var otpExists = await otpCache.OtpExistsAsync(request.ConfirmForgotPasswordDto.Email);
        if (!otpExists)
        {
            response.Errors.Add(new ErrorDetail
                { Field = "Otp", Message = "Otp đã hết hạn hoặc không có yêu cầu đổi mật khẩu" });
            response.Message = "Otp đã hết hạn hoặc không có yêu cầu đổi mật khẩu";
            response.StatusCode = 422;
            response.Success = false;
            return response;
        }

        var otp = await otpCache.GetOtpAsync(request.ConfirmForgotPasswordDto.Email);
        if (otp != request.ConfirmForgotPasswordDto.Otp)
        {
            response.Errors.Add(new ErrorDetail { Field = "Otp", Message = "Otp không hợp lệ" });
            response.Message = "Otp không hợp lệ";
            response.StatusCode = 422;
            response.Success = false;
            return response;
        }

        // Bắt lỗi cho mật khẩu
        if (request.ConfirmForgotPasswordDto.NewPassword.Length < 8)
            response.Errors.Add(new ErrorDetail
            {
                Message = "Mật khẩu phải có ít nhất 8 ký tự.",
                Field = "password"
            });

        if (request.ConfirmForgotPasswordDto.NewPassword != request.ConfirmForgotPasswordDto.ConfirmPassword)
            response.Errors.Add(new ErrorDetail
            {
                Message = "Mật khẩu mới và xác nhận mật khẩu không khớp.",
                Field = "re-password"
            });
        if (response.Errors.Count != 0)
        {
            response.Message = "Dữ liệu không hợp lệ.";
            response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            response.Success = false;
            return response;
        }

        try
        {
            // Get user by email to update password
            var user = await userRepository.GetByPropertyAsync(u => u.Email == request.ConfirmForgotPasswordDto.Email);
            // Check user exists
            if (user == null)
            {
                response.Errors.Add(new ErrorDetail { Field = "Email", Message = "Email không tồn tại" });
                response.Message = "Email không tồn tại";
                response.StatusCode = 404;
                response.Success = false;
                return response;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ConfirmForgotPasswordDto.NewPassword);
            // Update user
            await userRepository.UpdateAsync(user.UserId, user);
            // Delete otp
            await otpCache.DeleteOtpAsync(request.ConfirmForgotPasswordDto.Email);
            // Response
            response.Message = "Cập nhật mật khẩu thành công";
            response.StatusCode = 200;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Errors.Add(new ErrorDetail { Field = "Otp", Message = ex.Message });
            response.Message = ex.Message;
            response.StatusCode = 500;
            response.Success = false;
            return response;
        }

        return response;
    }
}