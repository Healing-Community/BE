using Grpc.Core;
using Application.Interfaces.Repository;
using GroupServiceGrpc;

namespace PRH_GroupService_API
{
    public class GroupGrpcService : GroupService.GroupServiceBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public GroupGrpcService(IGroupRepository groupRepository, IUserGroupRepository userGroupRepository)
        {
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        // Check if a Group exists
        public override async Task<CheckGroupResponse> CheckGroupExists(CheckGroupRequest request, ServerCallContext context)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            var response = new CheckGroupResponse
            {
                Exists = group != null // true if the group exists
            };
            return response;
        }

        // Check if a User belongs to a Group
        public override async Task<CheckUserInGroupResponse> CheckUserInGroup(CheckUserInGroupRequest request, ServerCallContext context)
        {
            var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);
            var response = new CheckUserInGroupResponse
            {
                IsMember = userGroup != null // true if the user is in the group
            };
            return response;
        }

        // Get details of a Group (including visibility)
        public override async Task<GetGroupDetailsResponse> GetGroupDetails(GetGroupDetailsRequest request, ServerCallContext context)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);

            if (group == null)
            {
                return new GetGroupDetailsResponse
                {
                    GroupId = request.GroupId,
                    Visibility = 1 // Default to private if the group does not exist
                };
            }

            return new GetGroupDetailsResponse
            {
                GroupId = group.GroupId,
                Visibility = group.GroupVisibility // 0: Public, 1: Private
            };
        }

        // Check if a User has access to a Group (public or member)
        public override async Task<CheckUserInGroupResponse> CheckUserInGroupOrPublic(CheckUserInGroupRequest request, ServerCallContext context)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);

            if (group == null)
            {
                return new CheckUserInGroupResponse
                {
                    IsMember = false
                };
            }

            // Allow access if GroupVisibility = 0 (Public) or user is a member of the group
            var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);
            var hasAccess = group.GroupVisibility == 0 || userGroup != null;

            return new CheckUserInGroupResponse
            {
                IsMember = hasAccess
            };
        }

        public override async Task<GetUserRoleInGroupResponse> GetUserRoleInGroup(GetUserRoleInGroupRequest request, ServerCallContext context)
        {
            var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);

            if (userGroup == null)
            {
                return new GetUserRoleInGroupResponse
                {
                    GroupId = request.GroupId,
                    UserId = request.UserId,
                    Role = "None" // Nếu không tồn tại trong nhóm
                };
            }

            return new GetUserRoleInGroupResponse
            {
                GroupId = request.GroupId,
                UserId = request.UserId,
                Role = userGroup.RoleInGroup
            };
        }
    }
}
