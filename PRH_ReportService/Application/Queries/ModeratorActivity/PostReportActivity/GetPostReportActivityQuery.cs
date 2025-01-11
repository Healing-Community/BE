using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.PostReportActivity;

public record GetPostReportActivityQuery : IRequest<BaseResponse<IEnumerable<ModeratePostReportActivity>>>;