using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordDto ForgotPasswordDto) : IRequest<BaseResponse<string>>;