using Application.Commands_Queries.Commands.Users.RefreshToken;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;
using PRH_UserService_API.Middleware;

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