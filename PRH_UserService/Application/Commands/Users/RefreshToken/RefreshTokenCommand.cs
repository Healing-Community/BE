using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.RefreshToken
{
    public record RefreshTokenCommand(TokenDto TokenDto) : IRequest<BaseResponse<TokenDto>>;
}
