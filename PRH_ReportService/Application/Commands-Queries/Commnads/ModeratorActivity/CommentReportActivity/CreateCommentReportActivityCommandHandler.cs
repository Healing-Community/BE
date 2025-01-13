using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.CommentReportActivity;

public class CreateCommentReportActivityCommandHandler(IMongoRepository<ModerateCommentReportActivity> repository) : IRequestHandler<CreateCommentReportActivityCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateCommentReportActivityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var activity = new ModerateCommentReportActivity
            {
                CommentId = request.BanCommentMessage.CommentId,
                Id = Ulid.NewUlid().ToString(),
                UserEmail = request.BanCommentMessage.UserEmail,
                UserId = request.BanCommentMessage.UserId,
                Content = request.BanCommentMessage.Content,
                IsApprove = request.BanCommentMessage.IsApprove,
                UserName = request.BanCommentMessage.UserName,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };

            await repository.Create(activity);

            return BaseResponse<string>.SuccessReturn();
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
