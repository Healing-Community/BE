using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUserProfile;

public record GetUserFollowingQuery : IRequest<BaseResponse<List<UserProfileDto>>>;
    