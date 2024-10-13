using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.ResetPassword;

public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDto) : IRequest<BaseResponse<string>>;