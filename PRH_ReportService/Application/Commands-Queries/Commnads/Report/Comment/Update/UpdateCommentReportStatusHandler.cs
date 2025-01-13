using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Comment.Update;

public class UpdateCommentReportStatusHandler(IMongoRepository<CommentReport> repository) : IRequestHandler<UpdateCommentReportStatus, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateCommentReportStatus request, CancellationToken cancellationToken)
    {
        try
        {
            var commentReport = await repository.GetByPropertyAsync(x => x.CommentId == request.CommentId);
            if (commentReport == null)
            {
                return BaseResponse<string>.NotFound("Bình luận này đã được duyệt");
            }
            commentReport.IsApprove = request.IsApprove;
            await repository.UpdateAsync(commentReport.Id, commentReport);
            return BaseResponse<string>.SuccessReturn("Duyệt bình luận thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
