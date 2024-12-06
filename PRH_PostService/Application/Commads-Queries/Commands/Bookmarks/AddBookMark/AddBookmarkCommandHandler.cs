using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commads_Queries.Commands.Bookmarks.AddBookMark;

public class AddBookmarkCommandHandler(IBookMarkRepository bookMarkRepository, IHttpContextAccessor accessor) : IRequestHandler<AddBookmarkCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var bookmarkinDb = await bookMarkRepository.GetByPropertyAsync(b => b.UserId == userId && b.Name == request.Name);
            if (bookmarkinDb != null)
            {
                return BaseResponse<string>.BadRequest("Bookmark đã tồn tại");
            }
            var bookmark = new Bookmark
            {
                UserId = userId,
                BookmarkId = Ulid.NewUlid().ToString(),
                Name = request.Name,
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };
            await bookMarkRepository.Create(bookmark);
            return BaseResponse<string>.SuccessReturn(bookmark.BookmarkId);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}