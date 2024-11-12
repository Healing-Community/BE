using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.ForgotPassword.SendOtpToEmail;

public record ForgotPasswordCommand(SendForgotPasswordDto ForgotPasswordDto) : IRequest<BaseResponse<string>>;