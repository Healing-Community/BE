using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.Posts.GetPrivatePost;
/// <summary>
/// Get Private Posts Only 
/// </summary>
public class GetsUserPostQueryHandler(IPostRepository repository, IHttpContextAccessor accessor) : IRequestHandler<GetsUserPostQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetsUserPostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<IEnumerable<PostRecommendDto>>.Unauthorized();
            }
            var posts = await repository.GetsPostByPropertyPagingAsync(p=>p.UserId == userId, request.PageNumber, request.PageSize);
            // Map Post to PostDto in a new list
            var data = posts.Select(post => new PostRecommendDto
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
