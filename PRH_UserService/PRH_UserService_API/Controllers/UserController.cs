using Application.Commands.Users.AddUser;
using Application.Commands.Users.DeleteUser;
using Application.Commands.Users.UpdateUser;
using Application.Commands.Users.LoginUser;
using Application.Commands.Users.RegisterUser;
using Application.Commands.Users.VerifyUser;
using Application.Commons.DTOs;
using Application.Queries.Users.GetUsers;
using Application.Queries.Users.GetUsersById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands.Users.Logout;
using Application.Commands.Users.ForgotPassword;
using Application.Commands.Users.ResetPassword;
using Microsoft.AspNetCore.Authorization;
using PRH_UserService_API.Middleware;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController(ISender sender) : ControllerBase
    {
        [Authorize(Roles= "Users")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await sender.Send(new GetUsersQuery());
            return response.ToActionResult();
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await sender.Send(new GetUsersByIdQuery(id));
            return response.ToActionResult();
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var response = await sender.Send(new CreateUserCommand(user));
            return response.ToActionResult();
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto user)
        {
            var updatedUser = await sender.Send(new UpdateUserCommand(id, user));
            return Ok(updatedUser);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await sender.Send(new DeleteUserCommand(id));
            return Ok(response);
        }
        [AllowAnonymousRefreshToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await sender.Send(new LoginUserCommand(loginDto));
            return Ok(response);
        }
        [AllowAnonymousRefreshToken]

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await sender.Send(new RegisterUserCommand(registerUserDto,baseUrl));
            return response.ToActionResult();
        }
        [Obsolete]
        [HttpGet("verify-user")]
        public async Task<IActionResult> VerifyUser(string Token)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await sender.Send(new VerifyUserCommand(Token));
            return Redirect($"{baseUrl}/swagger");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = await sender.Send(new ForgotPasswordCommand(forgotPasswordDto));
            return response.ToActionResult();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var response = await sender.Send(new ResetPasswordCommand(resetPasswordDto));
            return response.ToActionResult();
        }
    }
}