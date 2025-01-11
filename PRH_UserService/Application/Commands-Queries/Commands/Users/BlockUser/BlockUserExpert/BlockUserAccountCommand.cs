using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.BlockUser;

public record BlockUserAccountCommand(string UserId, int Status) :IRequest<BaseResponse<string>>;