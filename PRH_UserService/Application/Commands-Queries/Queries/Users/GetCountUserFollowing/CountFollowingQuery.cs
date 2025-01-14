using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountUserFollowing
{
    public record CountFollowingQuery(string UserId) : IRequest<BaseResponse<FollowingCountDto>>;

}
