using System;
using Application.Commons;
using MediatR;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace Application.Commads_Queries.Commands.Bookmarks.DeleteBookmark;

public record DeleteBookmarkCommand(string BookmarkId) : IRequest<BaseResponse<string>>; 