using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.UserFollower;

public class FollowUserCommandHandler(IUserRepository userRepository, IMapper mapper, IFollowerRepository followerRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher) : IRequestHandler<FollowUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();
            var user = await userRepository.GetByIdAsync(userId);
            var followUser = await userRepository.GetByIdAsync(request.FollowUserDto.FollowerId);

            if (userId == request.FollowUserDto.FollowerId) return BaseResponse<string>.BadRequest(message: "Không thể theo dõi chính mình");
            if (user == null || followUser == null) return BaseResponse<string>.NotFound();
            if (await followerRepository.CheckFollow(userId, request.FollowUserDto.FollowerId)) return BaseResponse<string>.BadRequest(message: "Đã theo dõi người dùng này");

            // Create follower
            var follower = new Follower(id: Ulid.NewUlid().ToString(), userId: userId, followerId: request.FollowUserDto.FollowerId);

            // Save follower
            await followerRepository.Create(follower);

            var createdFollower = await followerRepository.GetByIdAsync(follower.Id);

            var followMessage = new FollowMessage
            {
                FollowerId = userId,
                FollowedUserId = request.FollowUserDto.FollowerId,
                FollowDate = createdFollower.CreatedAt,
                FollowedUserName = followUser.UserName,
                FollowingUserName = user.UserName
            };

            // Publish follow message to queue
            await messagePublisher.PublishAsync(followMessage, QueueName.FollowQueue, cancellationToken);

            return BaseResponse<string>.SuccessReturn("Theo dõi người dùng thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(message: e.Message);
        }
    }
}

