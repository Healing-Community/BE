using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.DeleteUser;

public record DeleteUserCommand(string Id) : IRequest<BaseResponse<string>>;