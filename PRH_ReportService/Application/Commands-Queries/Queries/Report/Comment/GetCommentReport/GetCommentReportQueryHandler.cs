using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.Comment.GetCommentReport;

public class GetCommentReportQueryHandler(IMongoRepository<CommentReport> commentReportRepository) : IRequestHandler<GetCommentReportQuery, BaseResponse<IEnumerable<CommentReport>>>
{
    public async Task<BaseResponse<IEnumerable<CommentReport>>> Handle(GetCommentReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var commentReports = await commentReportRepository.GetsAsync();
            return BaseResponse<IEnumerable<CommentReport>>.SuccessReturn(commentReports);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<CommentReport>>.InternalServerError(e.Message);
            throw;
        }
    }
}
