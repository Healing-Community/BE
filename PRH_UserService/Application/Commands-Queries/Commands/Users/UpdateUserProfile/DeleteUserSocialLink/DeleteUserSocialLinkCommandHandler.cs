using System;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.DeleteUserSocialLink;

public class DeleteUserSocialLinkCommandHandler(ISocialLinkRepository socialLinkRepository) : IRequestHandler<DeleteUserSocialLinkCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeleteUserSocialLinkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var item in request.PlatformNames)
            {
                var link = await socialLinkRepository.GetByPropertyAsync(u => u.PlatformName == item);
                if (link != null)
                {
                    await socialLinkRepository.DeleteAsync(link.LinkId);
                }else
                {
                    return BaseResponse<string>.NotFound($"Không tìm thấy link mạng xã hội {item}");
                }
            }
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
        return BaseResponse<string>.SuccessReturn("Xóa thành công");    
    }
}
