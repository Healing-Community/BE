﻿using Application.Commands.Users.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        var response = await sender.Send(refreshTokenCommand);
        return response.ToActionResult();
    }
}