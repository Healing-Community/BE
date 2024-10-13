using Application.Commons;
using MediatR;

namespace Application.Commands.Users.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<BaseResponse<string>>;