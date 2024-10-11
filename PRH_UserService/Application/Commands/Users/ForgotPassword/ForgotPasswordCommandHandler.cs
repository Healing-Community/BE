using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
namespace Application.Commands.Users.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IEmailRepository _emailRepository;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository, IEmailRepository emailRepository)
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

                await _emailRepository.SendEmailAsync(user.Email, "Reset your password", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");

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
}
