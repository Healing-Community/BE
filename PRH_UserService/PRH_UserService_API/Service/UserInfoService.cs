using System;
using Application.Commands_Queries.Queries.Users.GetUserProfile;
using Application.Interfaces.Repository;
using Domain.Entities;
using Grpc.Core;
using UserInformation;

namespace PRH_UserService_API.Service;

public class UserInfoService(ISender sender, IFollowerRepository followerRepository) : UserInfo.UserInfoBase
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
    public override async Task<ListFollowerResponse> GetListFollower(UserInfoRequest request, ServerCallContext context)
    {
        try
        {
            // Gửi query để lấy danh sách người theo dõi
            var followers = await followerRepository.GetFollowers(request.UserId);

            // Debug log để kiểm tra số lượng follower
            Console.WriteLine($"GetListFollower: {followers.Count()}");

            // Chuyển đổi danh sách người theo dõi sang danh sách FollowerResponse
            var followerResponses = followers.Select(follower => new FollowerResponse
            {
                FollowId = follower.UserId // Assuming `FollowId` là thuộc tính của từng đối tượng trong danh sách followers
            }).ToList();

            // Trả về danh sách đã chuyển đổi
            return new ListFollowerResponse
            {
                Followers = { followerResponses } // gRPC `repeated` chấp nhận kiểu danh sách IEnumerable hoặc List
            };

        }
        catch (Exception ex)
        {
            // Log lỗi và trả về response rỗng hoặc tùy chọn xử lý lỗi
            Console.WriteLine($"Error in GetListFollower: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to fetch followers"));
        }
    }

    private class FollowerInfo
    {
        public string UserId { get; set; }
        public string FollowerId { get; set; }
    }
}
