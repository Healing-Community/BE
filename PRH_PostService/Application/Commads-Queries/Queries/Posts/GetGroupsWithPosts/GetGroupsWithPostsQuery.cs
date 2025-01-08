using Application.Commons;
using Application.Commons.DTOs;
using MediatR;


namespace Application.Commads_Queries.Queries.Posts.GetGroupsWithPosts
{
    public record GetGroupsWithPostsQuery : IRequest<BaseResponse<IEnumerable<GroupWithPostCountDto>>>;
}
