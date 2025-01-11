using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Commands.Comments.BanComment;

public record BanCommentCommand(string CommentId) : IRequest<BaseResponse<string>>;
