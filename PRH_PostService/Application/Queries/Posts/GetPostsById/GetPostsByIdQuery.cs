using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Posts.GetPostsById
{
    public class GetPostsByIdQuery : IRequest<BaseResponse<PostDto>>
    {
        public string Id { get; }

        public GetPostsByIdQuery(string id)
        {
            Id = id;
        }
    }
}
