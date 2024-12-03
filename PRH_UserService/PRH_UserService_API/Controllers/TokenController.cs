using Application.Commands_Queries.Commands.Users.RefreshToken;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshToken)
    {
        var response = await sender.Send(new RefreshTokenCommand(refreshToken));
        return response.ToActionResult();
    }
}