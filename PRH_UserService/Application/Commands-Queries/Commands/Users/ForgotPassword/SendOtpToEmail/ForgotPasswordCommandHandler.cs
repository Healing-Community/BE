using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Redis;
using Application.Interfaces.Services;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.ForgotPassword.SendOtpToEmail;

public class ForgotPasswordCommandHandler(IOtpCache otpCache, IEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.Now
        };
        var otp = Authentication.GenerateOtp();
        var expriredTime = TimeSpan.FromMinutes(5);

        try
        {
            var isOtpExists = await otpCache.OtpExistsAsync(request.ForgotPasswordDto.Email);
            if (isOtpExists)
            {
                var isDeleted = await otpCache.DeleteOtpAsync(request.ForgotPasswordDto.Email);
                if (!isDeleted)
                {
                    response.Message = "Có lỗi xảy ra khi xóa OTP cũ.";
                    response.StatusCode = 500;
                    response.Success = false;
                    return response;
                }
            }

            await otpCache.SaveOtpAsync(request.ForgotPasswordDto.Email, otp, expriredTime);
            await emailService.SendOtpEmailAsync(request.ForgotPasswordDto.Email, otp);

            response.Message = "OTP đã được gửi đến email của bạn.";
            response.StatusCode = 200;
            response.Success = true;
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}