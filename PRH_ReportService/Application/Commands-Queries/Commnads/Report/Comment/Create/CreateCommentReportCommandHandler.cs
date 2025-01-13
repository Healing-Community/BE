using System;
using Application.Commons;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.Report.Comment;

public class CreateCommentReportCommandHandler(IMongoRepository<CommentReport> commentReportRepository) : IRequestHandler<CreateCommentReportCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateCommentReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var commentReport = new CommentReport
            {
                Id = Ulid.NewUlid().ToString(),
                UserId = request.CommentReportMessage.UserId,
                CommentId = request.CommentReportMessage.CommentId,
                Content = request.CommentReportMessage.Content,
                ReportedUserEmail = request.CommentReportMessage.ReportedUserEmail,
                ReportedUserId = request.CommentReportMessage.ReportedUserId,
                ReportedUserName = request.CommentReportMessage.ReportedUserName,
                UserEmail = request.CommentReportMessage.UserEmail,
                UserName = request.CommentReportMessage.UserName,
                PostId = request.CommentReportMessage.PostId,
                IsApprove = null,
                ReportTypeEnum = request.CommentReportMessage.ReportTypeEnum,
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
