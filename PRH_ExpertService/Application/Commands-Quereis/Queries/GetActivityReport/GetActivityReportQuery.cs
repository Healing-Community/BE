using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetActivityReport
{
    public class GetActivityReportQuery : IRequest<BaseResponse<IEnumerable<ActivityReportDTO>>>
    {
    }
}
