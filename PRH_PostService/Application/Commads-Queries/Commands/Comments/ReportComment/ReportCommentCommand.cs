using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Commands.Comments.ReportComment;

public record ReportCommentCommand(ReportCommentDto ReportCommentDto) : IRequest<BaseResponse<string>>;
