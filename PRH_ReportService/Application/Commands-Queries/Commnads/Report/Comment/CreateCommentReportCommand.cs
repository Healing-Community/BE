using Application.Commons;
using Domain.Constants.AMQPMessage;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Comment;

public record CreateCommentReportCommand(CommentReportMessage CommentReportMessage) : IRequest<BaseResponse<string>>;
