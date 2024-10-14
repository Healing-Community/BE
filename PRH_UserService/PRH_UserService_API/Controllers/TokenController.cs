using Application.Commands.Users.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;
using PRH_UserService_API.Middleware;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController(ISender sender) : ControllerBase
{
    [AllowAnonymousRefreshToken]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await sender.Send(command);
        return response.ToActionResult();
    }
}