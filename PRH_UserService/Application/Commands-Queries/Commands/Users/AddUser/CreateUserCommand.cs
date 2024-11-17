using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.AddUser;

public record CreateUserCommand(UserDto UserDto) : IRequest<BaseResponse<string>>;