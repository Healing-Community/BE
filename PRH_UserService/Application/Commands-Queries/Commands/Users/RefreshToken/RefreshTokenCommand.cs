using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.RefreshToken;

public record RefreshTokenCommand(RefreshTokenDto RefreshTokenDto) : IRequest<BaseResponse<TokenDto>>;