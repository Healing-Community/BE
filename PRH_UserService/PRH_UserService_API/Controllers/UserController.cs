using Application.Commands_Queries.Commands.Users.AddUser;
using Application.Commands_Queries.Commands.Users.DeleteUser;
using Application.Commands_Queries.Commands.Users.LoginUser;
using Application.Commands_Queries.Commands.Users.Logout;
using Application.Commands_Queries.Commands.Users.RegisterUser;
using Application.Commands_Queries.Commands.Users.ResetPassword;
using Application.Commands_Queries.Commands.Users.UpdateUser;
using Application.Commands_Queries.Commands.Users.UpdateUserProfile;
using Application.Commands_Queries.Commands.Users.UpdateUserProfile.DeleteUserSocialLink;
using Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateProfilePicture;
using Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateSocialMediaLink;
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
public class UserController(ISender sender, IHttpContextAccessor accessor) : ControllerBase
{
    //[Authorize(Roles = "User")]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var response = await sender.Send(new GetUsersQuery());
        return response.ToActionResult();
    }
    [Obsolete("This method is deprecated, please use get-user-profile method instead.")]
    [Authorize(Roles = "User, Expert, Admin, Moderator")]
    [HttpGet("get-by-id/{userId}")]
    private async Task<IActionResult> GetById(string userId)
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
    [Obsolete]
    [HttpPost("create")]
    private async Task<IActionResult> AddUser(UserDto user)
    {
        var response = await sender.Send(new CreateUserCommand(user));
        return response.ToActionResult();
    }
    [Authorize(Roles = "User, Expert, Admin, Moderator")]
    [HttpPut("update-user-profile")]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserDto user)
    {
        var response = await sender.Send(new UpdateUserProfileCommand(user));
        return response.ToActionResult();
    }
    /// <summary>
    /// Cập nhật link mạng xã hội của người dùng nếu nhập mới sẽ thêm mới, nếu trùng tên nền tảng sẽ cập nhật link mới
    /// </summary>
    /// <param name="socialLinkDtos"></param>
    /// <returns></returns>
    [HttpPut("update-social-media-link")]
    private async Task<IActionResult> UpdateSocialMediaLink(List<SocialLinkDto> socialLinkDtos)
    {
        var response = await sender.Send(new UpdateUserSocialMediaLinkCommand(socialLinkDtos));
        return response.ToActionResult();
    }
    /// <summary>
    /// Xóa link mạng xã hội dựa vào tên nền tảng. Ví dụ: [Facebook, Instagram, Twitter]
    /// </summary>
    /// <returns>Trạng thái</returns>
    /// <response code="200">Xóa thành công</response>
    /// <response code="404">Không tìm thấy link mạng xã hội</response>
    [HttpDelete("delete-social-media-link")]
    private async Task<IActionResult> DeleteSocialMediaLink(string[] platformNames)
    {
        var response = await sender.Send(new DeleteUserSocialLinkCommand(platformNames));
        return response.ToActionResult();
    }
    /// <summary>
    /// Cập nhật ảnh đại diện của người dùng dựa vào file ảnh được upload
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    [HttpPut("update-profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile formFile)
    {
        var response = await sender.Send(new UpdateProfilePictureCommand(formFile));
        return response.ToActionResult();
    }
    [Obsolete]
    [HttpPut("update/{id}")]
    private async Task<IActionResult> UpdateUser(string id, UserDto user)
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
        var request = accessor.HttpContext?.Request;
        //var baseUrl = $"{request?.Scheme}://{request?.Host}";
        var baseUrl = $"{Request.Scheme}://localhost:8000/user";
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
