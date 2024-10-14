using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.LoginUser;

public record LoginUserCommand(LoginDto LoginDto) : IRequest<BaseResponse<TokenDto>>;