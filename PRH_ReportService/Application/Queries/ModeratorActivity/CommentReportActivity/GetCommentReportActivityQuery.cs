using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.CommentReportActivity;

public record GetCommentReportActivityQuery : IRequest<BaseResponse<IEnumerable<ModerateCommentReportActivity>>>;