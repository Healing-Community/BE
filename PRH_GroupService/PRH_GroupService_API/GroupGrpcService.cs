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

        public override async Task<CheckGroupResponse> CheckGroupExists(CheckGroupRequest request, ServerCallContext context)
        {
            // Kiểm tra nếu group với group_id tồn tại trong cơ sở dữ liệu
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            var response = new CheckGroupResponse
            {
                Exists = group != null // true nếu group tồn tại, ngược lại false
            };
            return response;
        }
        public override async Task<CheckUserInGroupResponse> IsUserInGroup(CheckUserInGroupRequest request, ServerCallContext context)
        {
            var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);
            var response = new CheckUserInGroupResponse
            {
                Exists = userGroup != null // true nếu User nằm trong Group, ngược lại false
            };
            return response;
        }
    }
}
