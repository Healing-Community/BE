using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UserFollower;

public record FollowUserCommand(FollowUserDto FollowUserDto) : IRequest<BaseResponse<string>> ;