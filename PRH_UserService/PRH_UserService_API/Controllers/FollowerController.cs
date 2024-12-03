using Application.Commands_Queries.Commands.Users.UserFollower;
using Application.Commands_Queries.Commands.Users.UserFollower.UnfollowUser;
using Application.Commands_Queries.Queries.Users.GetUserProfile;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-following")]
        public async Task<IActionResult> GetUserFollowing(string userId)
        {
            var response = await sender.Send(new GetUserFollowingQuery(userId));
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
