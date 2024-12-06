using System;
using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commads_Queries.Commands.Bookmarks.AddBookmarkPost;

public record AddBookmarkPostCommand(BookmarkPostDto BookmarkPostDto) : IRequest<BaseResponse<string>>;