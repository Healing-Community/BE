using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetPostsInGroupByGroupId
{
    public record GetPostsInGroupByGroupIdQuery(string GroupId) : IRequest<BaseResponse<List<PostGroupWithoutGroupIdDto>>>;
}
