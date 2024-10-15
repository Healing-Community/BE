using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.Users.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
{
    private readonly IEmailRepository _emailRepository;
    private readonly IJwtTokenRepository _jwtTokenRepository;
    private readonly IUserRepository _userRepository;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository,
        IEmailRepository emailRepository)
    {
        _userRepository = userRepository;
        _jwtTokenRepository = jwtTokenRepository;
        _emailRepository = emailRepository;
    }

    public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.ForgotPasswordDto.Email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Email not found.";
                return response;
            }

            var resetToken = _jwtTokenRepository.GenerateToken(user);
            var resetLink = $"https://healingcommunity.com/reset-password?token={resetToken}";

            // Prepare email content
            string emailContent = $@"
                <html>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f0f4f8;'>
                    <div style='max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);'>
                        <div style='text-align: center;'>
                            <img src='https://example.com/path-to-image.jpg' alt='Healing Image' style='max-width: 100%; height: auto; border-radius: 8px;'/>
                        </div>
                        <h2 style='color: #4caf50; text-align: center; margin-top: 20px;'>Đặt lại mật khẩu của bạn</h2>
                        <p style='font-size: 17px; line-height: 1.8; color: #444; text-align: justify;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng nhấp vào liên kết dưới đây để đặt lại mật khẩu của bạn:</p>
                        <div style='text-align: center; margin-top: 20px;'>
                            <a href='{resetLink}' style='background-color: #4caf50; color: #ffffff; padding: 12px 30px; border-radius: 5px; text-decoration: none; font-size: 18px;'>Đặt lại mật khẩu</a>
                        </div>
                        <p style='font-size: 14px; color: #666; text-align: center; margin-top: 30px;'>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                        <p style='text-align: center; color: #999; font-size: 13px;'>&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";

            // Send verification email
            await _emailRepository.SendEmailAsync(user.Email, "Đặt lại mật khẩu của bạn", emailContent);

            response.Success = true;
            response.Message = "Password reset email sent successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Failed to send password reset email.";
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}