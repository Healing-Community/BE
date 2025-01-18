using Application.Commands.GroupRequests.ApproveGroupRequest;
using Application.Commands.ManageGroup.ApproveUser;
using Application.Commands.ManageGroup.AssignRole;
using Application.Commands.ManageGroup.RemoveMember;
using Application.Commons.Tools;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_GroupService_API.Extentions;
using System.Reflection;
using System.Security.Claims;


namespace PRH_GroupService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageGroupController(ISender _sender) : ControllerBase
    {
        [HttpPost("assign-role")]
        [Authorize]
        public async Task<IActionResult> AssignRole(string groupId, string userId, string role)
        {
            if (!Authentication.IsUserInRole(HttpContext, "Admin", "Moderator"))
            {
                return Forbid("Chỉ có chủ nhóm mới có quyền gắn role thành viên.");
            }
            var response = await _sender.Send(new AssignRoleCommand(groupId, userId, role, HttpContext));
            return response.ToActionResult();
        }
        [HttpPost("approve-user-join-group")]
        [Authorize]
        public async Task<IActionResult> ApproveUser(string queueId, bool isApproved)
        {
            var response = await _sender.Send(new ApproveUserCommand(queueId, isApproved, HttpContext));
            return response.ToActionResult();
        }
        [HttpDelete("remove-member")]
        [Authorize]
        public async Task<IActionResult> RemoveMember(string groupId, string memberUserId)
        {
            var response = await _sender.Send(new RemoveMemberCommand(groupId, memberUserId, HttpContext));
            return response.ToActionResult();
        }
        /// <summary>
        /// Mod hoặc Admin dùng để phê duyệt yêu cầu tạo nhóm
        /// </summary>
        /// <param name="groupRequestId"></param>
        /// <param name="isApproved"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("approve-request-create-group/{groupRequestId}")]
        public async Task<IActionResult> ApproveGroupRequest(string groupRequestId, bool isApproved)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _sender.Send(new ApproveGroupRequestCommand(groupRequestId, isApproved, userId));
            return response.ToActionResult();
        }
    }
}
