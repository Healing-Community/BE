using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Groups.AddGroup
{
    public class CreateGroupCommandHandler(IGroupRepository groupRepository, IUserGroupRepository userGroupRepository) 
        : IRequestHandler<CreateGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
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
                if (userId == null )
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }
                if (!Authentication.IsUserInRole(request.HttpContext, "Admin", "Moderator"))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Message = "Chỉ Admin hoặc Moderator mới có quyền tạo nhóm.";
                    return response;
                }


                var group = new Group
                {
                    GroupId = Ulid.NewUlid().ToString(),
                    GroupName = request.GroupDto.GroupName,
                    Description = request.GroupDto.Description,
                    AvatarGroup = request.GroupDto.AvatarGroup,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7),
                    CreatedByUserId = userId,
                    IsAutoApprove = request.GroupDto.IsAutoApprove,
                    GroupVisibility = request.GroupDto.GroupVisibility,
                    MemberLimit = request.GroupDto.MemberLimit,
                    CurrentMemberCount = 1
                };

                await groupRepository.Create(group);

                // Thêm record vào UserGroup để chủ nhóm là thành viên mặc định
                var userGroup = new UserGroup
                {
                    GroupId = group.GroupId,
                    UserId = userId,
                    RoleInGroup = "Owner",
                    JoinedAt = DateTime.UtcNow.AddHours(7)
                };
                await userGroupRepository.Create(userGroup);

                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Tạo nhóm thành công";
                response.Data = group.GroupId;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Tạo nhóm thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
