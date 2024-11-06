using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.Posts.UpdatePost
{
    public class UpdatePostCommandHandler(IPostRepository postRepository)
        : IRequestHandler<UpdatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.postId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var existingPost = await postRepository.GetByIdAsync(request.postId);
                var updatedPost = new Post
                {
                    PostId = request.postId,
                    CategoryId = request.PostDto.CategoryId,
                    Title = request.PostDto.Title,
                    CoverImgUrl = request.PostDto.CoverImgUrl,
                    VideoUrl = request.PostDto.VideoUrl,
                    Description = request.PostDto.Description,
                    Status = request.PostDto.Status,
                    CreateAt = existingPost.CreateAt,
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                };
                await postRepository.Update(request.postId, updatedPost);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Cập nhật bài viết thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Cập nhật bài viết thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
