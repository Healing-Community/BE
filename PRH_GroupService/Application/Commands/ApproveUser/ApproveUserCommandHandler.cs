using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System.Net;
using Domain.Enum;

namespace Application.Commands.ApproveUser
{
    public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, BaseResponse<string>>
    {
        private readonly IApprovalQueueRepository _approvalQueueRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public ApproveUserCommandHandler(
            IApprovalQueueRepository approvalQueueRepository,
            IGroupRepository groupRepository,
            IUserGroupRepository userGroupRepository)
        {
            _approvalQueueRepository = approvalQueueRepository;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<string>> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.QueueId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập.";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Lấy thông tin hàng chờ phê duyệt
                var queueEntry = await _approvalQueueRepository.GetByIdAsync(request.QueueId);
                if (queueEntry == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy yêu cầu với QueueId: {request.QueueId}.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Lấy thông tin nhóm
                var group = await _groupRepository.GetByIdAsync(queueEntry.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Nhóm không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Kiểm tra quyền của người dùng
                if (group.CreatedByUserId != userId)
                {
                    var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(group.GroupId, userId);
                    // Kiểm tra nếu người dùng không thuộc nhóm hoặc không có quyền Owner/Moderator
                    if (userGroup == null ||
                        (userGroup.RoleInGroup != RoleInGroup.Owner.ToString() &&
                         userGroup.RoleInGroup != RoleInGroup.Moderator.ToString()))
                    {
                        response.Success = false;
                        response.Message = "Bạn không có quyền phê duyệt thành viên.";
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return response;
                    }
                }

                if (request.IsApproved)
                {
                    // Thêm người dùng vào nhóm
                    var userGroup = new UserGroup
                    {
                        GroupId = queueEntry.GroupId,
                        UserId = queueEntry.UserId,
                        RoleInGroup = RoleInGroup.User.ToString(),
                        JoinedAt = DateTime.UtcNow.AddHours(7)
                    };
                    await _userGroupRepository.Create(userGroup);

                    // Tăng CurrentMemberCount trong Group
                    group.CurrentMemberCount++;
                    await _groupRepository.Update(group.GroupId, group);

                    response.Message = "Thành viên đã được phê duyệt.";
                }
                else
                {
                    response.Message = "Yêu cầu phê duyệt thành viên đã bị từ chối.";
                }

                // Xóa hàng chờ
                await _approvalQueueRepository.DeleteAsync(request.QueueId);

                response.Success = true;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi !!! Không thể xử lý yêu cầu.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
