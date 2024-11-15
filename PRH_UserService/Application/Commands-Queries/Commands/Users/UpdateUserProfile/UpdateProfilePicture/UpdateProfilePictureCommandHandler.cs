using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateProfilePicture;

public class UpdateProfilePictureCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateProfilePictureCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        string base64Picture = string.Empty;
        try
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BaseResponse<string>.Unauthorized();
            }
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BaseResponse<string>.NotFound();
            }
            var profilePicture = request.ProfilePicture;
            // profilePicture to base64 string
            base64Picture = Tools.ConvertImageToBase64(profilePicture,200,200,9);

            if (profilePicture == null)
            {
                return BaseResponse<string>.NotFound("Vui lòng chọn ảnh đại diện");
            }
            else
            {
                user.ProfilePicture = base64Picture;
                await userRepository.UpdateAsync(userId,user);
            }

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
        return BaseResponse<string>.SuccessReturn(base64Picture);
    }
}
