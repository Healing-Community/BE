using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage.Report;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commads_Queries.Commands.Posts.BanPost_Moderator;

public class BanPostCommandHandler(IGrpcHelper grpcHelper, IHttpContextAccessor accessor, IMessagePublisher messagePublisher, IPostRepository postRepository) : IRequestHandler<BanPostCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BanPostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                return BaseResponse<string>.NotFound();
            }
            if (post.Status == (int)PostStatus.Baned)
            {
                return BaseResponse<string>.SuccessReturn("Bài viết đã bị ban trước đó");
            }
            post.Status = (int)PostStatus.Baned;


            if (request.IsApprove)
            {
                await postRepository.Update(post.PostId, post);
            }

            // Tạo message qua report service để đồng bộ dữ liệu

            await messagePublisher.PublishAsync(new SyncBanPostReportMessage
            {
                PostId = post.PostId,
                IsApprove = request.IsApprove
            }, QueueName.SyncPostReportQueue, cancellationToken);


            // Grpc to user service to get user email (moderator)
            var reportedUserReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );

            if (reportedUserReply == null)
            {
                return BaseResponse<string>.NotFound();
            }

            var message = new BanPostMessage
            {
                PostId = post.PostId,
                UserEmail = reportedUserReply.Email,
                UserId = userId,
                UserName = reportedUserReply.UserName,
                PostTitle = post.Title,
                Reason = request.IsApprove ? "Ban bài viết do vi phạm qui định nền tảng" : "Không ban bài viết do không vi phạm qui định nền tảng",
                IsApprove = request.IsApprove
            };
            // Tạo message qua report service admin xem hành động của moderate
            await messagePublisher.PublishAsync(message, QueueName.BanPostQueue, cancellationToken);

            return BaseResponse<string>.SuccessReturn();

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
