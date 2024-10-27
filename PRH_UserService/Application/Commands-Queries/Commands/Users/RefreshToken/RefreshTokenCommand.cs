using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.RefreshToken;

public record RefreshTokenCommand(RefreshTokenDto RefreshTokenDto, HttpContext HttpContext) : IRequest<BaseResponse<TokenDto>>;