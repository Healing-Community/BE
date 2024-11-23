using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.UserGroups.JoinGroups
{
    public class JoinGroupCommandHandler(
        IGroupRepository groupRepository, 
        IUserGroupRepository userGroupRepository,
        IApprovalQueueRepository approvalQueueRepository)
        : IRequestHandler<JoinGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra xem nhóm có tồn tại không
                var group = await groupRepository.GetByIdAsync(request.UserGroupDto.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy nhóm với ID: {request.UserGroupDto.GroupId}.";
                    response.StatusCode = 404;
                    return response;
                }

                // Check if the group has reached its member limit
                if (group.CurrentMemberCount >= group.MemberLimit)
                {
                    response.Success = false;
                    response.Message = "Nhóm đã đạt giới hạn thành viên.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Kiểm tra xem người dùng đã tham gia nhóm chưa
                var userGroupExists = await userGroupRepository.GetByGroupAndUserIdAsync(request.UserGroupDto.GroupId, userId);
                if (userGroupExists != null)
                {
                    response.Success = false;
                    response.Message = "Người dùng đã tham gia nhóm này rồi.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }


                if (!group.IsAutoApprove)
                {
                    // Add the user to the approval queue
                    var approvalQueue = new ApprovalQueue
                    {
                        QueueId = Ulid.NewUlid().ToString(),
                        GroupId = request.UserGroupDto.GroupId,
                        UserId = userId,
                        RequestedAt = DateTime.UtcNow.AddHours(7),
                        IsApproved = false
                    };

                    await approvalQueueRepository.Create(approvalQueue);

                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Yêu cầu gia nhập đã được gửi. Chờ phê duyệt từ quản trị viên.";
                }
                else
                {
                    // Auto-approve the user
                    var userGroup = new UserGroup
                    {
                        GroupId = request.UserGroupDto.GroupId,
                        UserId = userId,
                        JoinedAt = DateTime.UtcNow
                    };

                    await userGroupRepository.Create(userGroup);

                    // Increment the group's member count
                    group.CurrentMemberCount++;
                    await groupRepository.Update(group.GroupId, group);

                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Tham gia nhóm thành công.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Không thể tham gia nhóm.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
