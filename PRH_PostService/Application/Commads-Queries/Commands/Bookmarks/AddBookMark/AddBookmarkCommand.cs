using System;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Commands.Bookmarks.AddBookMark;

public record AddBookmarkCommand(string Name) : IRequest<BaseResponse<string>>;