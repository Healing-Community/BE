using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.Logout
{
    public record LogoutUserCommand(LogoutRequestDto LogoutRequestDto) : IRequest<BaseResponse<string>>;
}
