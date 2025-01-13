using System;
using Application.Commons;
using Domain.Entities.ModeratorActivity;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.PostReportActivity;

public class CreatePostReportActivityCommandHandler(IMongoRepository<ModeratePostReportActivity> repository) : IRequestHandler<CreatePostReportActivityCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreatePostReportActivityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var postReportActivity = new ModeratePostReportActivity
            {
                Id =  Ulid.NewUlid().ToString(),
                PostId = request.BanPostMessage.PostId,
                PostTitle = request.BanPostMessage.PostTitle,
                UserId = request.BanPostMessage.UserId,
                UserName = request.BanPostMessage.UserName,
                UserEmail = request.BanPostMessage.UserEmail,
                Reason = request.BanPostMessage.Reason,
                IsApprove = request.BanPostMessage.IsApprove,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };

            await repository.Create(postReportActivity);

            return BaseResponse<string>.SuccessReturn();
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
