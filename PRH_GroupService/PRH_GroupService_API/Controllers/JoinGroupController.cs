using Application.Commands.JoinGroups;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_GroupService_API.Extentions;

namespace PRH_GroupService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoinGroupController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup([FromBody] UserGroupDto userGroupDto)
        {
            var response = await sender.Send(new JoinGroupCommand(userGroupDto, HttpContext));
            return response.ToActionResult();
        }
    }
}
