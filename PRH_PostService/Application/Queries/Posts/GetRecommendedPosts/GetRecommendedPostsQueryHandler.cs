using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

public class GetRecommendedPostsQueryHandler(IPostRepository repository, IHttpContextAccessor accessor) : IRequestHandler<GetRecommendedPostsQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetRecommendedPostsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                var randomPosts = await repository.GetRandomPostsAsync(request.PageNumber, request.PageSize);
                return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(randomPosts.Select(post => new PostRecommendDto
                {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    VideoUrl = post.VideoUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                }));
            }
            var posts = await repository.GetRecommendedPostsAsync(userId ?? string.Empty, request.PageNumber, request.PageSize);
            // Map Post to PostDto in a new list
            var data = posts.Select(post => new PostRecommendDto
            {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    VideoUrl = post.VideoUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                });
            return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(data);
        }
        catch (Exception ex) 
        {
           return BaseResponse<IEnumerable<PostRecommendDto>>.InternalServerError(ex.Message);
        }
    }
}
