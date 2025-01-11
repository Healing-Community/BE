using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Comment.Update;

public record UpdateCommentReportStatus(string CommentId, bool IsApprove) : IRequest<BaseResponse<string>>;