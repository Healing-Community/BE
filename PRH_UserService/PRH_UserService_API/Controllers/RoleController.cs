﻿using Application.Queries.Roles.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ISender _iSender;
        public RoleController(ISender sender)
        {
            _iSender = sender;
        }

        [HttpGet("Gets")]
        public async Task<IActionResult> Get() 
        {
            var roles = await _iSender.Send(new GetRolesQuery());
            return Ok(roles);
        }


    }
}