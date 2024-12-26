using Application.Commands.ManageGroup.AssignRole;
using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;
using System.Net;

namespace Application.Commands.Groups.AssignRole
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, BaseResponse<string>>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;

        public AssignRoleCommandHandler(IUserGroupRepository userGroupRepository, IGroupRepository groupRepository)
        {
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<string>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId của người thực hiện
                var currentUserId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (currentUserId == null)
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền truy cập.";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Lấy thông tin nhóm
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy nhóm.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Lấy thông tin role của người thực hiện
                var currentUserGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, currentUserId);
                if (currentUserGroup == null)
                {
                    response.Success = false;
                    response.Message = "Bạn không thuộc nhóm này.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                if (currentUserGroup.RoleInGroup == RoleInGroup.Moderator.ToString())
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền gắn role cho thành viên khác.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                // Kiểm tra nếu người dùng muốn phân quyền cho chính mình
                if (request.UserId == currentUserId)
                {
                    response.Success = false;
                    response.Message = "Bạn không thể tự phân quyền cho chính mình.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Lấy thông tin thành viên cần phân quyền
                var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);
                if (userGroup == null)
                {
                    response.Success = false;
                    response.Message = "Người dùng không thuộc nhóm.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Kiểm tra và gán role
                if (Enum.TryParse(request.Role, true, out RoleInGroup newRole) &&
                    (newRole == RoleInGroup.Moderator || newRole == RoleInGroup.User))
                {
                    await _userGroupRepository.UpdateRole(request.GroupId, request.UserId, newRole);

                    response.Success = true;
                    response.Message = $"Phân quyền thành công. Vai trò mới: {newRole}.";
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Vai trò không hợp lệ.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi khi phân quyền.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
