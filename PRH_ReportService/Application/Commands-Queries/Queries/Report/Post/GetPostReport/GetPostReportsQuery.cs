using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.GetPostReport;

public record GetPostReportsQuery : IRequest<BaseResponse<IEnumerable<PostReport>>>;
