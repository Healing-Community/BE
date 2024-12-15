using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Bookmarks.GetsPostBookmark;

public class GetsPostBookmarkHandler(IBookMarkRepository bookmarkRepository,IPostRepository postRepository, IBookmarkPostRepository bookmarkPostRepository, IHttpContextAccessor accessor) : IRequestHandler<GetsPostBookmark, BaseResponseCount<IEnumerable<PostDetailDto>>>
{
    public async Task<BaseResponseCount<IEnumerable<PostDetailDto>>> Handle(GetsPostBookmark request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponseCount<IEnumerable<PostDetailDto>>.Unauthorized();
            var bookmarkInDb = await bookmarkRepository.GetByPropertyAsync(b => b.UserId == userId && b.BookmarkId == request.BookmarkId);
            if (bookmarkInDb == null) return BaseResponseCount<IEnumerable<PostDetailDto>>.SuccessReturn([]);
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
            int totalPost = postDetailDtos?.Count() ?? 0;
            return BaseResponseCount<IEnumerable<PostDetailDto>>.SuccessReturn(postDetailDtos,total:totalPost);
        }
        catch (Exception e)
        {
            return BaseResponseCount<IEnumerable<PostDetailDto>>.InternalServerError(e.Message);
        }
    }
}