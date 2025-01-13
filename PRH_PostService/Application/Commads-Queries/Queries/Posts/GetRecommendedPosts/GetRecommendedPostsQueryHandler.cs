using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
/// <summary>
/// Get Public Posts Query Handler (Not get private posts) 
/// </summary>
/// <param name="repository"></param>
/// <param name="accessor"></param>
public class GetRecommendedPostsQueryHandler(IPostRepository repository, IHttpContextAccessor accessor) : IRequestHandler<GetRecommendedPostsQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetRecommendedPostsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Not login
            if (string.IsNullOrEmpty(userId))
            {
                var randomPosts = await repository.GetRandomPostsAsync(request.PageNumber, request.PageSize);
                return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(randomPosts.Where(post=>post.Status == (int)PostStatus.Public).Select(post => new PostRecommendDto
                {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                }));
            }
            // Get recommended posts for user logined
            var posts = await repository.GetRecommendedPostsAsync(userId ?? string.Empty, request.PageNumber, request.PageSize);
            // Map Post to PostDto in a new list
            var data = posts.Where(p=>p.Status == (int)PostStatus.Public).Select(post => new PostRecommendDto
            {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
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
