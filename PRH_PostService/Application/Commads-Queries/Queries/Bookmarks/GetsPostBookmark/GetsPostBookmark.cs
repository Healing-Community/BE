using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Queries.Bookmarks.GetsPostBookmark;

public record GetsPostBookmark(string BookmarkId) : IRequest<BaseResponse<IEnumerable<PostDetailDto>>>;
