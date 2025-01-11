using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Queries.ModeratorActivity.CommentReportActivity;

public class GetCommentReportActivityQueryHandler(IMongoRepository<ModerateCommentReportActivity> repository) : IRequestHandler<GetCommentReportActivityQuery, BaseResponse<IEnumerable<ModerateCommentReportActivity>>>
{
    public async Task<BaseResponse<IEnumerable<ModerateCommentReportActivity>>> Handle(GetCommentReportActivityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activities = await repository.GetsAsync();

            return BaseResponse<IEnumerable<ModerateCommentReportActivity>>.SuccessReturn(activities);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<ModerateCommentReportActivity>>.InternalServerError(e.Message);
            throw;
        }
    }
}
