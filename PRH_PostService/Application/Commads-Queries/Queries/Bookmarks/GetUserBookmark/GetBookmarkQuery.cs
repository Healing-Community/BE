using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

public record GetBookmarkQuery : IRequest<BaseResponse<IEnumerable<Bookmark>>>;