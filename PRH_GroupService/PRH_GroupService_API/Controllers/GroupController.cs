using Application.Commands.Groups.AddGroup;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_GroupService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(GroupDto group)
        {
            var response = await sender.Send(new CreateGroupCommand(group, HttpContext));
            return response.ToActionResult();
        }
    }
}
