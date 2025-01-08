using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.Comment.GetCommentReport;

public record GetCommentReportQuery : IRequest<BaseResponse<IEnumerable<CommentReport>>>;

