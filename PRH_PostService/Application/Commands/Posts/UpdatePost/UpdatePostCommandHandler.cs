using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Posts.UpdatePost
{
    public class UpdatePostCommandHandler(IPostRepository postRepository, ICategoryRepository categoryRepository) 
        : IRequestHandler<UpdatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.postId,
                Timestamp = DateTime.UtcNow
            };
            try
            {
                var existingPost = await postRepository.GetByIdAsync(request.postId);
                var updatedPost = new Post
                {
                    Id = request.postId,
                    CategoryId = request.PostDto.CategoryId,
                    Title = request.PostDto.Title,
                    CoverImgUrl = request.PostDto.CoverImgUrl,
                    VideoUrl = request.PostDto.VideoUrl,
                    Description = request.PostDto.Description,
                    Status = request.PostDto.Status,
                    UpdateAt = DateTime.UtcNow,
                };
                await postRepository.Update(request.postId, updatedPost);
                response.Success = true;
                response.Message = "Post updated successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update post";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }
    }
}
