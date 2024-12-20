using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Share;

public class GetSharePostQueryHandler(IShareRepository shareRepository,IPostRepository postRepository, IHttpContextAccessor accessor) : IRequestHandler<GetSharePostQuery, BaseResponse<IEnumerable<PostDetailDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostDetailDto>>> Handle(GetSharePostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await shareRepository.GetsByPropertyAsync(s=> s.UserId == userId && s.Platform == "Internal");
            var postList = posts?.ToList();
            foreach (var item in postList ?? [])
            {
                item.Post = await postRepository.GetByIdAsync(item.PostId);
            }
            var postDetailDtos = postList?.Select(p => new PostDetailDto
            {
                PostId = p.PostId,
                Title = p.Post.Title,
                CategoryId = p.Post.CategoryId,
                UserId = p.Post.UserId,
                CoverImgUrl = p.Post.CoverImgUrl,
                CreateAt = p.Post.CreateAt,
                Description = p.Post.Description,
                GroupId = p.Post.GroupId,
                Status = p.Post.Status,
                UpdateAt = p.Post.UpdateAt
            });
            if (postDetailDtos == null) return BaseResponse<IEnumerable<PostDetailDto>>.SuccessReturn(new List<PostDetailDto>());
            return BaseResponse<IEnumerable<PostDetailDto>>.SuccessReturn(postDetailDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostDetailDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
