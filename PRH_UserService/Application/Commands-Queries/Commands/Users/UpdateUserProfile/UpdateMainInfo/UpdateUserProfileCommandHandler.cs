using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
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
            var socialLinks = await socialLinkRepository.GetsByPropertyAsync(p => p.UserId == userId);
            var dtoSocialLinks = new Dictionary<string, string>
            {
                { "Facebook", request.UserDto.SocialLink.Facebook },
                { "Instagram", request.UserDto.SocialLink.Instagram },
                { "Twitter", request.UserDto.SocialLink.Twitter },
                { "LinkedIn", request.UserDto.SocialLink.LinkedIn }
            };

            // Process each platform in the DTO
            foreach (var dtoLink in dtoSocialLinks)
            {
                var platformName = dtoLink.Key;
                var newUrl = dtoLink.Value;

                var existingSocialLink = socialLinks?.FirstOrDefault(p => p.PlatformName == platformName);

                // Add new social link if it doesn't exist and the URL is not empty
                if (existingSocialLink == null && !string.IsNullOrWhiteSpace(newUrl))
                {
                    await socialLinkRepository.Create(new SocialLink
                    {
                        LinkId = Ulid.NewUlid().ToString(),
                        PlatformName = platformName,
                        Url = newUrl,
                        UserId = userId
                    });
                }
                // Update existing social link if the URL has changed
                else if (existingSocialLink != null && existingSocialLink.Url != newUrl)
                {
                    existingSocialLink.Url = newUrl;
                    await socialLinkRepository.UpdateAsync(existingSocialLink.LinkId, existingSocialLink);
                }
            }

            // Optional: Delete links for platforms not in the DTO or with empty URLs
            var platformsToKeep = dtoSocialLinks
                .Where(dto => !string.IsNullOrWhiteSpace(dto.Value))
                .Select(dto => dto.Key)
                .ToHashSet();

            var linksToDelete = socialLinks
                .Where(link => !platformsToKeep.Contains(link.PlatformName))
                .ToList();

            foreach (var linkToDelete in linksToDelete)
            {
                await socialLinkRepository.DeleteAsync(linkToDelete.LinkId);
            }
        }
        catch
        {
            return DetailBaseResponse<string>.InternalServerError();
        }
        return DetailBaseResponse<string>.SuccessReturn("Thông tin cá nhân đã được cập nhật", string.Empty);
    }
}
