﻿using Application.Queries.Roles.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("gets")]
    public async Task<IActionResult> Get()
    {
        var roles = await sender.Send(new GetRolesQuery());
        return Ok(roles);
    }
}