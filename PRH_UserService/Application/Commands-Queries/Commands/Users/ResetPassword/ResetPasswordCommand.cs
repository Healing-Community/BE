using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.ResetPassword;

public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDto, HttpContext HttpContext)
    : IRequest<DetailBaseResponse<string>>;