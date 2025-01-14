using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.SystemReport;

public class GetsUserSystemReportQueryHandler(IMongoRepository<UserReportSystem> repository) : IRequestHandler<GetsUserSystemReportQuery, BaseResponse<IEnumerable<UserReportSystem>>>
{
    public async Task<BaseResponse<IEnumerable<UserReportSystem>>> Handle(GetsUserSystemReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userReportSystem = await repository.GetsAsync();
            return BaseResponse<IEnumerable<UserReportSystem>>.SuccessReturn(userReportSystem);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<UserReportSystem>>.InternalServerError(e.Message);
            throw;
        }
    }
}
