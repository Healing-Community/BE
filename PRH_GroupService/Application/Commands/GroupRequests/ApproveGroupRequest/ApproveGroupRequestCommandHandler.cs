using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;


namespace Application.Commands.GroupRequests.ApproveGroupRequest
{
    public class ApproveGroupRequestCommandHandler : IRequestHandler<ApproveGroupRequestCommand, BaseResponse<string>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupCreationRequestRepository _requestRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public ApproveGroupRequestCommandHandler(
            IGroupRepository groupRepository,
            IGroupCreationRequestRepository requestRepository,
            IUserGroupRepository userGroupRepository)
        {
            _groupRepository = groupRepository;
            _requestRepository = requestRepository;
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<string>> Handle(ApproveGroupRequestCommand request, CancellationToken cancellationToken)
        {
            // Lấy yêu cầu tạo nhóm
            var existingRequest = await _requestRepository.GetByIdAsync(request.GroupRequestId);
            if (existingRequest == null)
                return BaseResponse<string>.NotFound("Yêu cầu không tồn tại.");

            // Nếu là phê duyệt
            if (request.IsApproved)
            {
                // Tạo nhóm mới
                var newGroup = new Group
                {
                    GroupId = Ulid.NewUlid().ToString(),
                    GroupName = existingRequest.GroupName,
                    Description = existingRequest.Description,
                    CreatedByUserId = request.ApprovedById,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    GroupVisibility = 1, 
                    CurrentMemberCount = 2, 
                    MemberLimit = 50 
                };

                await _groupRepository.Create(newGroup);

                // Thêm chủ nhóm (Admin/Moderator)
                var owner = new UserGroup
                {
                    GroupId = newGroup.GroupId,
                    UserId = request.ApprovedById,
                    RoleInGroup = "Owner",
                    JoinedAt = DateTime.UtcNow.AddHours(7)
                };
                await _userGroupRepository.Create(owner);

                // Thêm người yêu cầu vào nhóm
                var requester = new UserGroup
                {
                    GroupId = newGroup.GroupId,
                    UserId = existingRequest.RequestedById,
                    RoleInGroup = "Moderator",
                    JoinedAt = DateTime.UtcNow.AddHours(7)
                };
                await _userGroupRepository.Create(requester);

                // Cập nhật trạng thái của yêu cầu
                existingRequest.IsApproved = true;
                existingRequest.ApprovedAt = DateTime.UtcNow.AddHours(7);
                existingRequest.ApprovedById = request.ApprovedById;

                await _requestRepository.Update(existingRequest.GroupRequestId, existingRequest);
                return BaseResponse<string>.SuccessReturn("Nhóm đã được tạo thành công.");
            }
            else
            {
                // Từ chối yêu cầu
                existingRequest.IsApproved = false;
                existingRequest.ApprovedAt = DateTime.UtcNow.AddHours(7);
                existingRequest.ApprovedById = request.ApprovedById;

                await _requestRepository.Update(existingRequest.GroupRequestId, existingRequest);
                return BaseResponse<string>.SuccessReturn("Yêu cầu tạo nhóm đã bị từ chối.");
            }
        }
    }

}
