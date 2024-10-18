using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.ReportPosts.GetReports
{
    public record GetReportsQuery : IRequest<BaseResponse<IEnumerable<Report>>>;
}
