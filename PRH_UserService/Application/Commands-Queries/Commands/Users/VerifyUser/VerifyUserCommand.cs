using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.VerifyUser;

public record VerifyUserCommand(string Token) : IRequest<BaseResponse<string>>;