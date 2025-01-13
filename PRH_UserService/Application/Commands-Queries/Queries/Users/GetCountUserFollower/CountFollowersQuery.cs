using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountUserFollower
{
    public record CountFollowersQuery(string UserId) : IRequest<BaseResponse<FollowerCountDto>>;

}
