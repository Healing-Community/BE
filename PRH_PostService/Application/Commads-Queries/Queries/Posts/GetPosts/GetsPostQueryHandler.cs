using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.Posts.GetPosts
{
    public class GetsPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetsPostQuery, BaseResponse<IEnumerable<PostDetailDto>>>
    {
        public async Task<BaseResponse<IEnumerable<PostDetailDto>>> Handle(GetsPostQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<PostDetailDto>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var posts = await postRepository.GetsAsync();

                if (!posts.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bài viết nào.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Data = posts.Select(post => new PostDetailDto
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
                });
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
