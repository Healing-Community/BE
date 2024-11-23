using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UserFollower.UnfollowUser;

public record UnfollowUserCommand(string UserId) : IRequest<BaseResponse<bool>>;