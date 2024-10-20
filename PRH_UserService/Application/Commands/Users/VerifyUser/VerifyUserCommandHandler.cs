using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using NUlid;

namespace Application.Commands.Users.VerifyUser;

public class VerifyUserCommandHandler(ITokenService tokenService, IUserRepository userRepository)
    : IRequestHandler<VerifyUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            if (!tokenService.ValidateToken(request.Token, out var userId))
                return new BaseResponse<string>
                {
                    Id = Ulid.NewUlid().ToString(),
                    Success = false,
                    Message = "Invalid or expired verification token.",
                    Timestamp = DateTime.UtcNow
                };

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                return new BaseResponse<string>
                {
                    Id = Ulid.NewUlid().ToString(),
                    Success = false,
                    Message = "User not found.",
                    Timestamp = DateTime.UtcNow
                };

            user.Status = 1;
            await userRepository.Update(user.UserId, user);

            response.Success = true;
            response.Message = "Email verified successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Failed to verify email.";
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}