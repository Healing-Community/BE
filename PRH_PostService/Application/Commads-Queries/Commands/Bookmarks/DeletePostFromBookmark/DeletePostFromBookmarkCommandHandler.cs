using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Commands.Bookmarks.DeletePostFromBookmark;

public class DeletePostFromBookmarkCommandHandler(IBookMarkRepository bookmarkRepository,IBookmarkPostRepository bookmarkPostRepository, IHttpContextAccessor accessor) : IRequestHandler<DeletePostFromBookmarkCommand, BaseResponse<string>>
{

    public async Task<BaseResponse<string>> Handle(DeletePostFromBookmarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookmark = await bookmarkRepository.GetByPropertyAsync(b => b.UserId == userId && b.BookmarkId == request.BookmarkPostDto.BookmarkId);
            if (bookmark == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bookmark");
            }
            var bookmarkPost = await bookmarkPostRepository.GetByPropertyAsync(b=>b.PostId == request.BookmarkPostDto.PostId && b.BookmarkId == bookmark.BookmarkId);
            if (bookmarkPost == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bookmark");
            }
            await bookmarkPostRepository.DeleteAsync(bookmarkPost.BookmarkPostId);
            return BaseResponse<string>.SuccessReturn("Xóa bookmark thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
