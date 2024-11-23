using Application.Commands.Groups.AddGroup;
using Application.Commands.Groups.DeleteGroup;
using Application.Commands.Groups.UpdateGroup;
using Application.Commands.ManageGroup.RemoveMember;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Queries.Groups.GetGroups;
using Application.Queries.Groups.GetGroupsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_GroupService_API.Extentions;

namespace PRH_GroupService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(ISender sender) : ControllerBase
    {

        [HttpGet("get-all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var response = await sender.Send(new GetGroupsQuery());
            return response.ToActionResult();
        }

        [HttpGet("get-by-group-id/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string groupId)
        {
            var response = await sender.Send(new GetGroupsByIdQuery(groupId));
            return response.ToActionResult();
        }

        [HttpPost("create-group")]
        [Authorize]
        public async Task<IActionResult> CreateGroup(GroupDto group)
        {
            if (!Authentication.IsUserInRole(HttpContext, "Admin", "Moderator"))
            {
                return Forbid("Chỉ Admin hoặc Moderator mới có quyền tạo nhóm.");
            }
            var response = await sender.Send(new CreateGroupCommand(group, HttpContext));
            return response.ToActionResult();
        }

        
        [HttpPut("update-group/{groupId}")]
        [Authorize]
        public async Task<IActionResult> UpdateGroup(string groupId, GroupDto group)
        {
            if (!Authentication.IsUserInRole(HttpContext, "Admin", "Moderator"))
            {
                return Forbid("Chỉ Admin hoặc Moderator mới có quyền cập nhật nhóm.");
            }
            var response = await sender.Send(new UpdateGroupCommand(groupId, group));
            return response.ToActionResult();
        }

        [HttpDelete("delete-group/{groupId}")]
        [Authorize]
        public async Task<IActionResult> DeleteGroup(string groupId)
        {
            if (!Authentication.IsUserInRole(HttpContext, "Admin", "Moderator"))
            {
                return Forbid("Chỉ Admin hoặc Moderator mới có quyền xóa nhóm.");
            }
            var response = await sender.Send(new DeleteGroupCommand(groupId));
            return response.ToActionResult();
        }
    }
}
