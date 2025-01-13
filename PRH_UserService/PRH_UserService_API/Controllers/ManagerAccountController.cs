using Application.Commands_Queries.Commands.Users.BlockUser;
using Application.Commands_Queries.Commands.Users.BlockUser.BlockModerator;
using Application.Commands_Queries.Commands.Users.CreateModerator;
using Application.Commands_Queries.Queries.Users.GetUserManager;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerAccountController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Nếu role là Admin thì lấy moderator, nếu là Moderator thì lấy user và expert
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("get-user-manager")]
        public async Task<IActionResult> GetUserManager()
        {
            var response = await sender.Send(new GetUserManagerQuery());
            return response.ToActionResult();
        }
        /// <summary>
        /// Status = 0: Khóa tài khoản, Status = 1: Mở khóa tài khoản
        /// </summary>
        /// <param name="registerModeratorAccountDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("create-moderator-account")]
        public async Task<IActionResult> CreateModeratorAccount([FromBody] RegisterModeratorAccountDto registerModeratorAccountDto)
        {
            var response = await sender.Send(new CreateModeratorAccountCommand(registerModeratorAccountDto));
            return response.ToActionResult();
        }
        [Authorize(Roles="Admin")]
        [HttpPut("change-status-moderate-account")]
        public async Task<IActionResult> BlockModeratorAccount([FromBody]BlockModeratorCommand blockModeratorCommand)
        {
            var response = await sender.Send(blockModeratorCommand);
            return response.ToActionResult();
        }
        /// <summary>
        /// Status = 0: Khóa tài khoản, Status = 1: Mở khóa tài khoản
        /// </summary>
        /// <param name="blockUserCommand"></param>
        /// <returns></returns>
        [Authorize(Roles="Moderator")]
        [HttpPut("change-status-user-account")]
        public async Task<IActionResult> BlockUserAccount([FromBody]BlockUserAccountCommand blockUserCommand)
        {
            var response = await sender.Send(blockUserCommand);
            return response.ToActionResult();
        }
    }
}
