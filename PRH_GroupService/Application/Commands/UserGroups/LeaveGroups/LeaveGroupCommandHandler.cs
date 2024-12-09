using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.UserGroups.LeaveGroups
{
    public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand, BaseResponse<string>>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;

        public LeaveGroupCommandHandler(IUserGroupRepository userGroupRepository, IGroupRepository groupRepository)
        {
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;  
        }

        public async Task<BaseResponse<string>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra xem người dùng có trong nhóm không
                var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, userId);
                if (userGroup == null)
                {
                    response.Success = false;
                    response.Message = "Người dùng không tham gia nhóm này.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                await _userGroupRepository.DeleteAsyncV2(request.GroupId, userId);

                // Cập nhật số lượng thành viên trong nhóm (Giảm đi 1)
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group != null)
                {
                    group.CurrentMemberCount--;  
                    await _groupRepository.UpdateAfterLeaving(group);  
                }

                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Rời khỏi nhóm thành công.";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Không thể rời khỏi nhóm.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
