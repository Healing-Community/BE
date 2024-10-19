using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Posts.GetPostsById
{
    public record GetPostsByIdQuery(Guid id) : IRequest<BaseResponse<Post>>;
}
