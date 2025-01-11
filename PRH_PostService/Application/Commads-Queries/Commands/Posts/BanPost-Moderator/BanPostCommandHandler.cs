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

public class BanPostCommandHandler(IGrpcHelper grpcHelper,IHttpContextAccessor accessor,IMessagePublisher messagePublisher,IPostRepository postRepository) : IRequestHandler<BanPostCommand, BaseResponse<string>>
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
                return BaseResponse<string>.SuccessReturn();
            }
            post.Status = (int)PostStatus.Baned;
            
            await postRepository.Update(post.PostId, post);

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
                Reason = "Bị ban do vi phạm quy định"
            };

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
