using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Bookmarks.GetsPostBookmark;

public class GetsPostBookmarkHandler(IBookMarkRepository bookmarkRepository,IPostRepository postRepository, IBookmarkPostRepository bookmarkPostRepository, IHttpContextAccessor accessor) : IRequestHandler<GetsPostBookmark, BaseResponse<IEnumerable<PostDetailDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostDetailDto>>> Handle(GetsPostBookmark request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<IEnumerable<PostDetailDto>>.Unauthorized();
            var bookmarkInDb = await bookmarkRepository.GetByPropertyAsync(b => b.UserId == userId);
            if (bookmarkInDb == null) return BaseResponse<IEnumerable<PostDetailDto>>.SuccessReturn([]);
            var bookmarkPost = await bookmarkPostRepository.GetsByPropertyAsync(bp => bp.BookmarkId == bookmarkInDb.BookmarkId);
            var bookmarkPostList = bookmarkPost?.ToList();
            foreach (var item in bookmarkPostList)
            {
                item.Post = await postRepository.GetByIdAsync(item.PostId);
            }
            var postDetailDtos = bookmarkPostList?.Select(bp => new PostDetailDto
            {
                PostId = bp.PostId,
                Title = bp.Post.Title,
                CategoryId = bp.Post.CategoryId,
                UserId = bp.Post.UserId,
                CoverImgUrl = bp.Post.CoverImgUrl,
                CreateAt = bp.Post.CreateAt,
                Description = bp.Post.Description,
                GroupId = bp.Post.GroupId,
                Status = bp.Post.Status,
                UpdateAt = bp.Post.UpdateAt
            });
            return BaseResponse<IEnumerable<PostDetailDto>>.SuccessReturn(postDetailDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostDetailDto>>.InternalServerError(e.Message);
        }
    }
}