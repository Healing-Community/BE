using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Commands.Bookmarks.DeleteBookmark;

public class DeleteBookmarkCommandHandler(IHttpContextAccessor accessor, IBookMarkRepository bookMarkRepository, IBookmarkPostRepository bookmarkPostRepository) : IRequestHandler<DeleteBookmarkCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeleteBookmarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookmark = await bookMarkRepository.GetByPropertyAsync(x => x.BookmarkId == request.BookmarkId && x.UserId == userId);
            if (bookmark == null)
            {
                return BaseResponse<string>.NotFound("Bookmark không tồn tại.");
            }
            var bookmarkPost = await bookmarkPostRepository.GetsByPropertyAsync(x => x.BookmarkId == bookmark.BookmarkId);
            if (bookmarkPost == null)
            {
                return BaseResponse<string>.NotFound("Bookmark không tồn tại.");
            }
            var bookmarkPostList = bookmarkPost.ToList();
            foreach (var item in bookmarkPostList)
            {
                await bookmarkPostRepository.DeleteAsync(item.BookmarkPostId);
            }
            await bookMarkRepository.DeleteAsync(bookmark.BookmarkId);
            return BaseResponse<string>.SuccessReturn("Xóa bookmark thành công.");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
            throw;
        }
    }
}
