using System;
using Application.Commons;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.Report;

public class CreatePostReportCommandHandler(IMongoRepository<PostReport> postRepository) : IRequestHandler<CreatePostReportCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreatePostReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var postReport = new PostReport
            {
                Id = Ulid.NewUlid().ToString(),
                PostTitle = request.PostReportMessage.PostTitle,
                ReportedUserEmail = request.PostReportMessage.ReportedUserEmail,
                ReportedUserId = request.PostReportMessage.ReportedUserId,
                ReportedUserName = request.PostReportMessage.ReportedUserName,
                UserEmail = request.PostReportMessage.UserEmail,
                UserName = request.PostReportMessage.UserName,
                PostId = request.PostReportMessage.PostId,
                UserId = request.PostReportMessage.UserId,
                ReportTypeEnum = request.PostReportMessage.ReportTypeEnum,
                IsApprove = null,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };
            await postRepository.Create(postReport);
            return  BaseResponse<string>.SuccessReturn();
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}
