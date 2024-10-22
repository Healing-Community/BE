using Application.Commons;
using MediatR;

namespace Application.Commands.Users.DeleteUser;

public record DeleteUserCommand(string Id) : IRequest<BaseResponse<string>>;