﻿using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.Posts.GetPostsById
{
    public class GetPostsByIdQueryHandler(IPostRepository postRepository)
        : IRequestHandler<GetPostsByIdQuery, BaseResponse<PostDetailDto>>
    {
        public async Task<BaseResponse<PostDetailDto>> Handle(GetPostsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<PostDetailDto>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var post = await postRepository.GetByIdAsync(request.Id);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bài viết.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Data = new PostDetailDto
                {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    GroupId = post.GroupId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                };

                response.Success = true;
                response.Message = "Lấy bài viết thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
