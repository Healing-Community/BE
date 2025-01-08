using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Comment;

public class CreateCommentReportCommandHandler(IMongoRepository<CommentReport> commentReportRepository) : IRequestHandler<CreateCommentReportCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateCommentReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var commentReport = new CommentReport
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.CommentReportMessage.UserId,
                CommentId = request.CommentReportMessage.CommentId,
                Content = request.CommentReportMessage.Content,
                PostId = request.CommentReportMessage.PostId,
                reportTypeEnum = request.CommentReportMessage.ReportTypeEnum,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };

            await commentReportRepository.Create(commentReport);

            return BaseResponse<string>.SuccessReturn(commentReport.Id); 
        }
        catch (Exception e) 
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
