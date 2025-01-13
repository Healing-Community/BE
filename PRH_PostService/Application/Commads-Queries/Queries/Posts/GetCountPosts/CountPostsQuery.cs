using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetCountPosts
{
    public record CountPostsQuery(string UserId) : IRequest<BaseResponse<PostCountDto>>;

}
