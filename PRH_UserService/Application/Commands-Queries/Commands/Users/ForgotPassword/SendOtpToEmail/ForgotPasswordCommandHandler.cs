using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Redis;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.ForgotPassword.SendOtpToEmail;

public class ForgotPasswordCommandHandler(IOtpCache otpCache, IEmailService emailService, IUserRepository userRepository)
    : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var otp = Authentication.GenerateOtp();
        var expriredTime = TimeSpan.FromMinutes(5);
        try
        {
            var user = await userRepository.GetUserByEmailAsync(request.ForgotPasswordDto.Email);
            if (user == null)
            {
                return BaseResponse<string>.NotFound("Email không tồn tại.");
            }
            var isOtpExists = await otpCache.OtpExistsAsync(request.ForgotPasswordDto.Email);
            if (isOtpExists)
            {
                var isDeleted = await otpCache.DeleteOtpAsync(request.ForgotPasswordDto.Email);
                if (!isDeleted)
                {
                    return BaseResponse<string>.InternalServerError("Có lỗi xảy ra khi xóa OTP cũ.");
                }
            }

            await otpCache.SaveOtpAsync(request.ForgotPasswordDto.Email, otp, expriredTime);
            await emailService.SendOtpEmailAsync(request.ForgotPasswordDto.Email, otp);

            return BaseResponse<string>.SuccessReturn("Mã OTP đã được gửi đến email của bạn.");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}