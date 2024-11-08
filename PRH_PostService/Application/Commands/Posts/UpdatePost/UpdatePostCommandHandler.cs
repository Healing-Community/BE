using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Commands.Posts.UpdatePost
{
    public class UpdatePostCommandHandler(
        IPostRepository postRepository, 
        ICategoryRepository categoryRepository, 
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<UpdatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.PostId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                var existingPost = await postRepository.GetByIdAsync(request.PostId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Bài viết không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                if (existingPost.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền cập nhật bài viết này";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                if (!await categoryRepository.ExistsAsync(request.PostDto.CategoryId))
                {
                    response.Success = false;
                    response.Message = "Danh mục không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                var updatedPost = new Post
                {
                    PostId = request.PostId,
                    UserId = existingPost.UserId,
                    CategoryId = request.PostDto.CategoryId,
                    Title = request.PostDto.Title,
                    CoverImgUrl = request.PostDto.CoverImgUrl,
                    VideoUrl = request.PostDto.VideoUrl,
                    Description = request.PostDto.Description,
                    Status = request.PostDto.Status,
                    CreateAt = existingPost.CreateAt,
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                };

                await postRepository.Update(request.PostId, updatedPost);
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
