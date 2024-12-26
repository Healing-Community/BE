using Grpc.Core;
using Application.Interfaces.Repository;
using GroupServiceGrpc;

namespace PRH_GroupService_API
{
    public class GroupGrpcService : GroupService.GroupServiceBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupGrpcService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
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
    }
}
