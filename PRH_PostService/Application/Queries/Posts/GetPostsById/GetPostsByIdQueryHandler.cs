using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                Errors = new List<string>()
            };
            try
            {
                var post = await postRepository.GetByIdAsync(request.id);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found";
                    response.Errors.Add("No post found with the provided ID.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = post;
                response.Success = true;
                response.Message = "Post retrieved successfully";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
