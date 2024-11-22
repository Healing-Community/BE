using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.UserFollower.UnfollowUser;

public class UnfollowUserCommandHandler(IHttpContextAccessor httpContextAccessor, IFollowerRepository followerRepository) : IRequestHandler<UnfollowUserCommand, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var follower = await followerRepository.GetByPropertyAsync(x => x.UserId == userId && x.FollowerId == request.UserId);

            if (follower == null) return BaseResponse<bool>.NotFound();

            await followerRepository.DeleteAsync(follower.Id);

            return BaseResponse<bool>.SuccessReturn(true, "Huỷ theo dõi thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<bool>.InternalServerError(message: e.Message);
        }
    }
}
