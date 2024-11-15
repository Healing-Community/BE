using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using MediatR;

public class GetUserProfileQueryHandler(IUserRepository userRepository,ISocialLinkRepository socialLinkRepository, IMapper mapper) : IRequestHandler<GetUserProfileQuery, BaseResponse<UserProfileDto>>
{
    public async Task<BaseResponse<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return BaseResponse<UserProfileDto>.NotFound("Không tìm thấy người dùng");
        }

        var userProfile = mapper.Map<UserProfileDto>(user);
        var socialLinks = await socialLinkRepository.GetsByPropertyAsync(x => x.UserId == request.UserId);
        userProfile.SocialLinks = mapper.Map<IEnumerable<SocialLinkDto>>(socialLinks);
        return BaseResponse<UserProfileDto>.SuccessReturn(userProfile);
    }
}