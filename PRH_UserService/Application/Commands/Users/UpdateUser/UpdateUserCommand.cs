using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.UpdateUser;

public record UpdateUserCommand(Guid Id, UserDto UserDto) : IRequest<BaseResponse<string>>;