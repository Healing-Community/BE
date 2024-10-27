using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.AddUser;

public record CreateUserCommand(UserDto UserDto) : IRequest<BaseResponse<string>>;