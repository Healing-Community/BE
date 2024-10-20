using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.Users.Logout;

public class LogoutUserCommandHandler(ITokenRepository tokenRepository)
    : IRequestHandler<LogoutUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            var userId =
                Authentication.GetUserIdFromHttpContext(request.LogoutRequestDto.context ??
                                                        throw new InvalidOperationException());
            var tokenUser = await tokenRepository.GetByPropertyAsync(t => t.UserId.ToString() == userId);
            var refreshToken = request.LogoutRequestDto.RefreshToken;

            if (tokenUser != null && tokenUser.RefreshToken == refreshToken)
            {
                await tokenRepository.DeleteAsync(tokenUser.UserId);
                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Logout successfully.";
            }
            else
            {
                response.StatusCode = 401;
                response.Success = false;
                response.Message = "Logout unsuccessfully.";
                response.Errors.Add("Refresh token not valid!");
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Success = false;
            response.Message = "An error occurred while logging out.";
            response.Errors.Add(ex.Message); // Add error message to the list
        }

        return response;
    }
}