using Application.Commands.GroupRequests.ApproveGroupRequest;
using Application.Commands.GroupRequests.CreateGroupRequest;
using Application.Commands.Groups.AddGroup;
using Application.Commands.Groups.DeleteGroup;
using Application.Commands.Groups.UpdateGroup;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Queries.GroupRequests.GetApprovalRequests;
using Application.Queries.GroupRequests.GetUserRequests;
using Application.Queries.Groups.GetApprovalQueue;
using Application.Queries.Groups.GetGroups;
using Application.Queries.Groups.GetGroupsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_GroupService_API.Extentions;
using System.Security.Claims;

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
        [Authorize]
        [HttpGet("approval-queue/{groupId}")]
        public async Task<IActionResult> GetApprovalQueue(string groupId)
        {
            var response = await sender.Send(new GetAllApprovalQueueQuery(groupId, HttpContext));
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
        /// <summary>
        /// User hoặc Expert dùng để tạo yêu cầu tạo nhóm
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-request-group")]
        public async Task<IActionResult> CreateGroupRequest(CreateGroupRequestDto request)
        {
            var response = await sender.Send(new CreateGroupRequestCommand(request.GroupName, request.Description));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy danh sách yêu cầu tạo nhóm của một người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-user-requests-create-group/{userId}")]
        public async Task<IActionResult> GetUserRequests(string userId)
        {
            var response = await sender.Send(new GetUserRequestsQuery(userId));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy danh sách yêu cầu cần phê duyệt
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-approval-requests-create-group")]
        public async Task<IActionResult> GetApprovalRequests()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role); 
            var response = await sender.Send(new GetApprovalRequestsQuery(userRole));
            return response.ToActionResult();
        }
    }
}
