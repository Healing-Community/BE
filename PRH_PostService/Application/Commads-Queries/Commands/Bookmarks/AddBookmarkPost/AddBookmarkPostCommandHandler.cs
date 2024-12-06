using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commads_Queries.Commands.Bookmarks.AddBookmarkPost;

public class AddBookmarkPostCommandHandler(IBookMarkRepository bookMarkRepository,IBookmarkPostRepository bookmarkPostRepository, IHttpContextAccessor accessor) : IRequestHandler<AddBookmarkPostCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(AddBookmarkPostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookmarkinDb = await bookMarkRepository.GetByPropertyAsync(b=> b.UserId == userId && b.BookmarkId == request.BookmarkPostDto.BookmarkId);
            if (bookmarkinDb == null)
            {
                return BaseResponse<string>.NotFound();
            }
            var bookmarkPost = new BookmarkPost
            {
                BookmarkId = request.BookmarkPostDto.BookmarkId,
                PostId = request.BookmarkPostDto.PostId,
                BookmarkPostId = Ulid.NewUlid().ToString(),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };

            var bookmarkPostIndb = await bookmarkPostRepository.GetByPropertyAsync(b => b.BookmarkId == bookmarkPost.BookmarkId && b.PostId == bookmarkPost.PostId);
            if (bookmarkPostIndb != null)
            {
                return BaseResponse<string>.BadRequest("Bài viết đã tồn tại trong bookmark");
            }
            await bookmarkPostRepository.Create(bookmarkPost);
            return BaseResponse<string>.SuccessReturn(bookmarkPost.BookmarkPostId);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}
