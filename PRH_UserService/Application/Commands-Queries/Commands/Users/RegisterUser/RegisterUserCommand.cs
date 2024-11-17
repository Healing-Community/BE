using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.RegisterUser;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto, string BaseUrl)
    : IRequest<DetailBaseResponse<string>>;