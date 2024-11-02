using Application.Commands_Queries.Commands.Users.ForgotPassword.ConfirmForgotPassword;
using Application.Commands_Queries.Commands.Users.ForgotPassword.SendOtpToEmail;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForgotPasswordController(ISender sender) : ControllerBase
{
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(SendForgotPasswordDto forgotPasswordDto)
    {
        var response = await sender.Send(new ForgotPasswordCommand(forgotPasswordDto));
        return response.ToActionResult();
    }

    [HttpPost("confirm-forgot-password")]
    public async Task<IActionResult> ConfirmForgotPassword(ConfirmForgotPasswordDto confirmForgotPasswordDto)
    {
        var response = await sender.Send(new ConfirmForgotPasswordCommand(confirmForgotPasswordDto));
        return response.ToActionResult();
    }
}