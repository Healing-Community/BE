using Application.Commands_Queries.Queries.Users.GetUserProfile;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUserFollowing;

public class GetUserFollowingQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserFollowingQuery, BaseResponse<List<UserProfileDto>>>
{
    public async Task<BaseResponse<List<UserProfileDto>>> Handle(GetUserFollowingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = request.UserId;
            var user = await userRepository.GetByIdAsync(userId ?? string.Empty);
            if (user == null) return BaseResponse<List<UserProfileDto>>.NotFound();
            var followings = await userRepository.GetsByPropertyAsync(x => x.Followers.Any(x => x.UserId == userId));
            return BaseResponse<List<UserProfileDto>>.SuccessReturn(followings?.ToList().Select(x => new UserProfileDto
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FullName,
                ProfilePicture = x.ProfilePicture,
                PhoneNumber = x.PhoneNumber,
                Descrtiption = x.Descrtiption,
            }).ToList() ?? []);
        }
        catch (Exception e)
        {
            return BaseResponse<List<UserProfileDto>>.InternalServerError(message: e.Message);
        }
    }
}
