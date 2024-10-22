using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Posts.GetPosts
{
    public record GetsPostQuery : IRequest<BaseResponse<IEnumerable<Post>>>;
}
