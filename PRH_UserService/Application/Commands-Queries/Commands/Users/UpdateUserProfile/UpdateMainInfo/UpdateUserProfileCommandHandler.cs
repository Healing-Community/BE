using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Domain.Entities;
using NUlid;
namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(ISocialLinkRepository socialLinkRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateUserProfileCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return DetailBaseResponse<string>.Unauthorized();
            }

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return DetailBaseResponse<string>.NotFound();
            }
            // Update user information
            user.ProfilePicture = request.UserDto.ProfilePictureUrl;
            user.FullName = request.UserDto.FullName;
            user.PhoneNumber = request.UserDto.PhoneNumber;
            user.Descrtiption = request.UserDto.Descrtiption;
            await userRepository.UpdateAsync(userId, user);
            // Update social link
            foreach (var item in request.UserDto.SocialLinks)
            {
                var link = await socialLinkRepository.GetByPropertyAsync(u => u.UserId == userId && u.PlatformName == item.PlatformName);
                if (item.PlatformName == link?.PlatformName)
                {
                    link.Url = item.Url;
                    await socialLinkRepository.UpdateAsync(link.LinkId, link);
                }
                else
                {
                    var socialLink = new SocialLink
                    {
                        LinkId = Ulid.NewUlid().ToString(),
                        PlatformName = item.PlatformName,
                        Url = item.Url,
                        UserId = userId
                    };
                    await socialLinkRepository.Create(socialLink);
                }
            }
        }
        catch
        {
            return new DetailBaseResponse<string>
            {
                Id = NewId.NextGuid().ToString(),
                Message = "Có lỗi xảy ra khi cập nhật thông tin cá nhân",
                Success = false
            };
        }
        return DetailBaseResponse<string>.SuccessReturn("Thông tin cá nhân đã được cập nhật", string.Empty);
    }
}
