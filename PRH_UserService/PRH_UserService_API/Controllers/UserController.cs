using Application.Commands_Queries.Commands.Users.AddUser;
using Application.Commands_Queries.Commands.Users.DeleteUser;
using Application.Commands_Queries.Commands.Users.LoginUser;
using Application.Commands_Queries.Commands.Users.Logout;
using Application.Commands_Queries.Commands.Users.RegisterUser;
using Application.Commands_Queries.Commands.Users.ResetPassword;
using Application.Commands_Queries.Commands.Users.UpdateUser;
using Application.Commands_Queries.Commands.Users.VerifyUser;
using Application.Commands_Queries.Queries.Users.GetUsers;
using Application.Commands_Queries.Queries.Users.GetUsersById;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;
using PRH_UserService_API.Middleware;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(ISender sender) : ControllerBase
{
    //[Authorize(Roles = "User")]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await sender.Send(new GetUsersQuery());
        return response.ToActionResult();
    }
    [Obsolete("This method is deprecated, please use get-user-profile method instead.")]
    [Authorize(Roles="User, Expert, Admin, Moderator")]
    [HttpGet("get-by-id/{userId}")]
    public async Task<IActionResult> GetById(string userId)
    {
        var response = await sender.Send(new GetUsersByIdQuery(userId));
        return response.ToActionResult();
    }
   // [Authorize(Roles="User, Expert, Admin, Moderator")]
    [HttpGet("get-user-profile/{userId}")]
    public async Task<IActionResult> GetUserProfile(string userId)
    {
        var response = await sender.Send(new GetUserProfileQuery(userId));
        return response.ToActionResult();
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddUser(UserDto user)
    {
        var response = await sender.Send(new CreateUserCommand(user));
        return response.ToActionResult();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(string id, UserDto user)
    {
        var response = await sender.Send(new UpdateUserCommand(id, user));
        return response.ToActionResult();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var response = await sender.Send(new DeleteUserCommand(id));
        return response.ToActionResult();
    }

    [@AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var response = await sender.Send(new LoginUserCommand(loginDto));
        return response.ToActionResult();
    }

    [@AllowAnonymous]
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
        //var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = await sender.Send(new VerifyUserCommand(token));
        if (!response.Success)
            return Redirect("https://nghia46.github.io/Static-Page-Healing-community/verification-failed");
        return Redirect("https://nghia46.github.io/Static-Page-Healing-community/success-verification");
    }

    [Authorize(Roles = "User")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
    {
        logoutRequestDto.context = HttpContext;
        var response = await sender.Send(new LogoutUserCommand(logoutRequestDto));
        return response.ToActionResult();
    }

    [Authorize(Roles = "User, Expert")]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var response = await sender.Send(new ResetPasswordCommand(resetPasswordDto, HttpContext));
        return response.ToActionResult();
    }
}