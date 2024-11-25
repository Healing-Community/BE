using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Queries.Reactions.GetReactionsById;
using MediatR;
using Microsoft.AspNetCore.Http;

public class GetRecommendedPostsQueryHandler(IPostRepository repository, IHttpContextAccessor accessor) : IRequestHandler<GetRecommendedPostsQuery, BaseResponse<IEnumerable<PostDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostDto>>> Handle(GetRecommendedPostsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await repository.GetRecommendedPostsAsync(userId ?? string.Empty, request.PageNumber, request.PageSize);
            // Map Post to PostDto in a new list
            var data = posts.Select(post => new PostDto
                {
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    VideoUrl = post.VideoUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                });
            return BaseResponse<IEnumerable<PostDto>>.SuccessReturn(data);
        }
        catch (Exception ex) 
        {
           return BaseResponse<IEnumerable<PostDto>>.InternalServerError(ex.Message);
        }
    }
}