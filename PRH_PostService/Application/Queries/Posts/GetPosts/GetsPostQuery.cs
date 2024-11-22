using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Posts.GetPosts
{
    public record GetsPostQuery : IRequest<BaseResponse<IEnumerable<PostDto>>>;
}
