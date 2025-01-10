using System;
using Grpc.Core;
using UserInformation;

namespace PRH_UserService_API.Service;

public class UserInfoService(ISender sender) : UserInfo.UserInfoBase
{
    public override async Task<UserInfoResponse> GetUserInfo(UserInfoRequest request, ServerCallContext context)
    {
        var response = await sender.Send(new GetUserProfileQuery(request.UserId));
        return new UserInfoResponse
        {
            UserName = response.Data?.UserName,
            Email = response.Data?.Email,
        };
    }
}
