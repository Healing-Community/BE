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

        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUserGroups()
        {
            var response = await _sender.Send(new GetUserGroupsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetUserGroupById(string groupId, string userId)
        {
            var response = await _sender.Send(new GetUserGroupByIdQuery(groupId, userId));
            return response.ToActionResult();
        }
    }
}
