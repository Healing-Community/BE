using Application.Commands.Users.AddUser;
using Application.Commands.Users.DeleteUser;
using Application.Commands.Users.ForgotPassword;
using Application.Commands.Users.LoginUser;
using Application.Commands.Users.Logout;
using Application.Commands.Users.RegisterUser;
using Application.Commands.Users.ResetPassword;
using Application.Commands.Users.UpdateUser;
using Application.Commands.Users.VerifyUser;
using Application.Commons.DTOs;
using Application.Queries.Users.GetUsers;
using Application.Queries.Users.GetUsersById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "User")]
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
        var response = await sender.Send(new UpdateUserCommand(id, user));
        return response.ToActionResult();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await sender.Send(new DeleteUserCommand(id));
        return response.ToActionResult();
    }

    [AllowAnonymousRefreshToken]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var response = await sender.Send(new LoginUserCommand(loginDto));
        return response.ToActionResult();
    }

    [AllowAnonymousRefreshToken]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = await sender.Send(new RegisterUserCommand(registerUserDto, baseUrl));
        return response.ToActionResult();
    }

    [Obsolete]
    [HttpGet("verify-user")]
    public async Task<IActionResult> VerifyUser(string token)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = await sender.Send(new VerifyUserCommand(token));
        return Redirect($"{baseUrl}/swagger");
    }

    [Authorize(Roles = "User")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
    {
        logoutRequestDto.context = HttpContext;
        var response = await sender.Send(new LogoutUserCommand(logoutRequestDto));
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