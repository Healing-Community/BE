using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Posts.GetPostsById
{
    public class GetPostsByIdQueryHandler(IPostRepository postRepository) : IRequestHandler<GetPostsByIdQuery, BaseResponse<Post>>
    {
        public async Task<BaseResponse<Post>> Handle(GetPostsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Post>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var post = await postRepository.GetByIdAsync(request.id);
                response.Data = post;
                response.Success = true;
                response.Message = "Post retrieved successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors = new[] { e.Message };
            }
            return response;
        }
    }
}
