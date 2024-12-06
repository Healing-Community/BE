using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Commands.Bookmarks.DeletePostFromBookmark;

public record DeletePostFromBookmarkCommand(BookmarkPostDto BookmarkPostDto) : IRequest<BaseResponse<string>>;