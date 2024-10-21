using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Posts.GetPostsById
{
    public record GetPostsByIdQuery(string id) : IRequest<BaseResponse<Post>>;
}
