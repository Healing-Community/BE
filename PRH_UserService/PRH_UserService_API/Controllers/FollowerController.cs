using Application.Commands_Queries.Commands.Users.UserFollower;
using Application.Commands_Queries.Commands.Users.UserFollower.UnfollowUser;
using Application.Commands_Queries.Queries.Users.GetUserProfile;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-following")]
        public async Task<IActionResult> GetUserFollowing()
        {
            var response = await sender.Send(new GetUserFollowingQuery());
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPost("follow-user")]
        public async Task<IActionResult> FollowUser(FollowUserDto followUserDto)
        {
            var response = await sender.Send(new FollowUserCommand(followUserDto));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpDelete("unfollow-user")]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var response = await sender.Send(new UnfollowUserCommand(userId));
            return response.ToActionResult();
        }
    }
}