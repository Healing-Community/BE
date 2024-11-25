using Application.Commands.ManageGroup.ApproveUser;
using Application.Commands.UserGroups.JoinGroups;
using Application.Commands.UserGroups.LeaveGroups;
using Application.Commons.DTOs;
using Application.Queries.UserGroups.GetUserGroups;
using Application.Queries.UserGroups.GetUserGroupsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_GroupService_API.Extentions;

namespace PRH_GroupService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly ISender _sender;

        public UserGroupController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("get-all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUserGroup()
        {
            var response = await _sender.Send(new GetUserGroupsQuery());
            return response.ToActionResult();
        }

        [HttpGet("get-by-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserGroupById(string groupId, string userId)
        {
            var response = await _sender.Send(new GetUserGroupByIdQuery(groupId, userId));
            return response.ToActionResult();
        }


        [HttpPost("join")]
        [Authorize(Roles = "User,Expert")]
        public async Task<IActionResult> JoinGroup([FromBody] UserGroupDto userGroupDto)
        {
            var response = await _sender.Send(new JoinGroupCommand(userGroupDto, HttpContext));
            return response.ToActionResult();
        }

        [HttpPost("leave")]
        [Authorize(Roles = "User,Expert")]
        public async Task<IActionResult> LeaveGroup(string groupId)
        {
            var response = await _sender.Send(new LeaveGroupCommand(groupId, HttpContext));
            return response.ToActionResult();
        }

    }
}
