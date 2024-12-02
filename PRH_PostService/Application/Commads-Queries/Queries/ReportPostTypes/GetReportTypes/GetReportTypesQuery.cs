using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.ReportPostTypes.GetReportTypes
{
    public record GetReportTypesQuery : IRequest<BaseResponse<IEnumerable<ReportType>>>;
}
