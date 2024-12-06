using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Bookmarks;

public class GetBookmarkQueryHandler(IBookMarkRepository bookMarkRepository, IHttpContextAccessor accessor) : IRequestHandler<GetBookmarkQuery, BaseResponse<IEnumerable<Bookmark>>>
{
    public async Task<BaseResponse<IEnumerable<Bookmark>>> Handle(GetBookmarkQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<IEnumerable<Bookmark>>.Unauthorized();
            var bookmark = await bookMarkRepository.GetsByPropertyAsync(b => b.UserId == userId);
            return BaseResponse<IEnumerable<Bookmark>>.SuccessReturn(bookmark ?? []);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<Bookmark>>.InternalServerError(e.Message);
            throw;
        }
    }
}
