using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.Users.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository) : IRequestHandler<ResetPasswordCommand, BaseResponse<string>>
{
    private readonly IJwtTokenRepository _jwtTokenRepository = jwtTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<BaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            if (request.ResetPasswordDto.NewPassword != request.ResetPasswordDto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match.";
                return response;
            }

            if (!_jwtTokenRepository.ValidateToken(request.ResetPasswordDto.Token, out var userId))
            {
                response.Success = false;
                response.Message = "Invalid or expired token.";
                return response;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
            await _userRepository.Update(user.UserId, user);

            response.Success = true;
            response.Message = "Password reset successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Failed to reset password.";
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}