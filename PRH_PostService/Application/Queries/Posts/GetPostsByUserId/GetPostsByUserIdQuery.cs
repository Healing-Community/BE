using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Queries.Posts.GetPostsByUserId
{
    public record GetPostsByUserIdQuery(string UserId) : IRequest<BaseResponse<IEnumerable<PostDetailDto>>>;
}
