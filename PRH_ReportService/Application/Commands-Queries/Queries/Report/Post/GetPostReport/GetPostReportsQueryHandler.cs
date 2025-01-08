using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.GetPostReport;

public class GetPostReportsQueryHandler(IMongoRepository<PostReport> postReportRepository) : IRequestHandler<GetPostReportsQuery, BaseResponse<IEnumerable<PostReport>>>
{
    public async Task<BaseResponse<IEnumerable<PostReport>>> Handle(GetPostReportsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // get all
            var postReports = await postReportRepository.GetsAsync();
            return BaseResponse<IEnumerable<PostReport>>.SuccessReturn(postReports);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostReport>>.InternalServerError(e.Message);
            throw;
        }
    }
}
