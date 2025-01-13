using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.BlockUser.BlockModerator;

public record BlockModeratorCommand(string UserId, int Status) :IRequest<BaseResponse<string>>;