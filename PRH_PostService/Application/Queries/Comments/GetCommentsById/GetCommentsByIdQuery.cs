using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Comments.GetCommentsById
{
    public record GetCommentsByIdQuery(string Id) : IRequest<BaseResponse<CommentDtoResponse>>;
}
