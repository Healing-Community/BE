using Application.Commands.UserGroups.JoinGroups;
using Application.Commands.UserGroups.LeaveGroups;
using Application.Commons.DTOs;
using Application.Queries.UserGroups.GetRoleInGroup;
using Application.Queries.UserGroups.GetRoleInGroupByGroupIdAndUserId;
using Application.Queries.UserGroups.GetUserGroups;
using Application.Queries.UserGroups.GetUserGroupsByGroupId;
using Application.Queries.UserGroups.GetUserGroupsById;
using Application.Queries.UserGroups.GetUserGroupsByUserId;
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

        [HttpGet("get-by-group-id-and-user-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserGroupByGroupIdAndUserId(string groupId, string userId)
        {
            var response = await _sender.Send(new GetUserGroupByIdQuery(groupId, userId));
            return response.ToActionResult();
        }

        [HttpGet("get-by-user-id/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserGroupsByUserId(string userId)
        {
            var response = await _sender.Send(new GetUserGroupsByUserIdQuery(userId));
            return response.ToActionResult();
        }

        [HttpGet("get-by-group-id/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserGroupsByGroupId(string groupId)
        {
            var response = await _sender.Send(new GetUserGroupsByGroupIdQuery(groupId));
            return response.ToActionResult();
        }

        [HttpGet("get-role-count-by-group-id/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoleInGroupByGroupId(string groupId)
        {
            var response = await _sender.Send(new GetRoleInGroupByGroupIdQuery(groupId));
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

        /// <summary>
        /// Lấy RoleInGroup của User trong một Group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("get-role-in-group")]
        [Authorize]
        public async Task<IActionResult> GetRoleInGroup([FromQuery] string userId, [FromQuery] string groupId)
        {
            var response = await _sender.Send(new GetRoleInGroupQuery(userId, groupId));
            return response.ToActionResult();
        }
    }
}
