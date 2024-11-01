using Application.Commands.Groups.AddGroup;
using Application.Commands.Groups.DeleteGroup;
using Application.Commands.Groups.RemoveMember;
using Application.Commands.Groups.UpdateGroup;
using Application.Commons.DTOs;
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
        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await sender.Send(new GetGroupsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await sender.Send(new GetGroupsByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(GroupDto group)
        {
            var response = await sender.Send(new CreateGroupCommand(group, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGroup(string id, GroupDto group)
        {
            var response = await sender.Send(new UpdateGroupCommand(id, group));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete-group/{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var response = await sender.Send(new DeleteGroupCommand(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("remove-member")]
        public async Task<IActionResult> RemoveMember(string groupId, string memberUserId)
        {
            var response = await sender.Send(new RemoveMemberCommand(groupId, memberUserId, HttpContext));
            return response.ToActionResult();
        }
    }
}
