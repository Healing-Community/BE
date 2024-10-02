using Application.Commands.Users.AddUser;
using Application.Commands.Users.LoginUser;
using Application.Commands.Users.RegisterUser;
using Application.Commands.Users.VerifyUser;
using Application.Commons.DTOs;
using Application.Queries.Users.GetUsers;
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

        [HttpGet("Gets")]
        public async Task<IActionResult> Get()
        {
            var users = await _iSender.Send(new GetUsersQuery());
            return Ok(users);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var addedUser = await _iSender.Send(new CreateUserCommand(user));
            return Ok(addedUser);
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
