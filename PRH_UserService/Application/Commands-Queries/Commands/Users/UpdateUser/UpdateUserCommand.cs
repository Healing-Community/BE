using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UpdateUser;

public record UpdateUserCommand(string Id, UserDto UserDto) : IRequest<BaseResponse<string>>;