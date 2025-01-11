using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage.Report;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commads_Queries.Commands.Comments.BanComment;

public class BanCommentCommandHandler(IMessagePublisher messagePublisher,IGrpcHelper grpcHelper,IHttpContextAccessor accessor, ICommentRepository commentRepository) : IRequestHandler<BanCommentCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BanCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var comment = await commentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                return BaseResponse<string>.NotFound();
            }
            // Override comment content
            comment.Content = "Bình luận này đã bị ban do vi phạm quy định";
            
            await commentRepository.Update(comment.CommentId, comment);

            // Grpc to user service to get user email (moderator)

            var userInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );

            if (userInfoReply == null)
            {
                return BaseResponse<string>.NotFound();
            }

            // Send message to user service to notify user that their comment has been banned
            var message = new BanCommentMessage{
                CommentId = comment.CommentId,
                UserId = userId,
                UserName = userInfoReply.UserName,
                UserEmail = userInfoReply.Email,
                Content = comment.Content
            };

            await messagePublisher.PublishAsync(message,QueueName.BanCommentQueue,cancellationToken);

            return BaseResponse<string>.SuccessReturn("Bình luận đã bị ban");

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
