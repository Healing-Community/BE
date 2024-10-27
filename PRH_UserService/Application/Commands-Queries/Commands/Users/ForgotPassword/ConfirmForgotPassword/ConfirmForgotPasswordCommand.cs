using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.ForgotPassword.ConfirmForgotPassword;

public record ConfirmForgotPasswordCommand(ConfirmForgotPasswordDto ConfirmForgotPasswordDto) : IRequest<DetailBaseResponse<string>>;
