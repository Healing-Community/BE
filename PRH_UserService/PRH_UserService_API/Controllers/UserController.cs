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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender _iSender;
        public UserController(ISender sender)
        {
            _iSender = sender;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var users = await _iSender.Send(new GetUsersQuery());
            return Ok(users);
        }
        [HttpGet("GetById/{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _iSender.Send(new GetUsersByIdQuery(id));
            return Ok(user);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var addedUser = await _iSender.Send(new CreateUserCommand(user));
            return Ok(addedUser);
        }
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto user)
        {
            var updatedUser = await _iSender.Send(new UpdateUserCommand(id, user));
            return Ok(updatedUser);
        }
        [HttpDelete("Delete/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _iSender.Send(new DeleteUserCommand(id));
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await _iSender.Send(new LoginUserCommand(loginDto));
            return Ok(response);
        }
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
        {
            var response = await _iSender.Send(new RegisterUserCommand(registerUserDto));
            return Ok(response);
        }
        [HttpPost("VerifyUser")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyUserCommand command)
        {
            var result = await _iSender.Send(command);
            return Ok(result);
        }
    }
}
