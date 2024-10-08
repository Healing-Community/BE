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

namespace Application.Commands.Posts.AddPost
{
    public class CreatePostCommandHandler(IPostRepository postRepository, ICategoryRepository categoryRepository) 
        : IRequestHandler<CreatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var postId = NewId.NextSequentialGuid();

            var post = new Post
            {
                Id = postId,
                CategoryId = request.PostDto.CategoryId,
                Title = request.PostDto.Title,
                CoverImgUrl = request.PostDto.CoverImgUrl,
                VideoUrl = request.PostDto.VideoUrl,
                Description = request.PostDto.Description,
                Status = request.PostDto.Status,
                CreateAt = DateTime.UtcNow,
            };

            var response = new BaseResponse<string>
            {
                Id = postId,
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                await postRepository.Create(post);
                response.Success = true;
                response.Errors = Enumerable.Empty<string>();
                response.Message = "Post created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors = new[] { ex.Message };
                response.Message = "Failed to create post";
            }

            return response;
        }
    }
}
