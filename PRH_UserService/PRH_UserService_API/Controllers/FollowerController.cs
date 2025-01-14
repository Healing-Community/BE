using Application.Commands_Queries.Commands.Users.UserFollower;
using Application.Commands_Queries.Commands.Users.UserFollower.UnfollowUser;
using Application.Commands_Queries.Queries.Users.GetCountUserFollower;
using Application.Commands_Queries.Queries.Users.GetCountUserFollowing;
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
        /// <summary>
        ///  Đếm số lượng người mà bạn đang theo dõi - không phân quyền login
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("count-followers/{userId}")]
        public async Task<IActionResult> CountFollowers(string userId)
        {
            var response = await sender.Send(new CountFollowersQuery(userId));
            return response.ToActionResult();
        }
        /// <summary>
        ///  Đếm số lượng người đang theo dõi bạn - không phân quyền login
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("count-following/{userId}")]
        public async Task<IActionResult> CountFollowing(string userId)
        {
            var response = await sender.Send(new CountFollowingQuery(userId));
            return response.ToActionResult();
        }
    }
}
