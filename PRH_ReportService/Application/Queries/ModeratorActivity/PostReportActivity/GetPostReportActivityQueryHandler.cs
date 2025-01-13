using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.PostReportActivity;

public class GetPostReportActivityQueryHandler(IMongoRepository<ModeratePostReportActivity> repository) : IRequestHandler<GetPostReportActivityQuery, BaseResponse<IEnumerable<ModeratePostReportActivity>>>
{
    public async Task<BaseResponse<IEnumerable<ModeratePostReportActivity>>> Handle(GetPostReportActivityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activities = await repository.GetsAsync();

            return BaseResponse<IEnumerable<ModeratePostReportActivity>>.SuccessReturn(activities);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<ModeratePostReportActivity>>.InternalServerError(e.Message);
            throw;
        }
    }
}

