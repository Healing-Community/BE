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
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.httpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }
                var group = new Group
                {
                    GroupId = Ulid.NewUlid().ToString(),
                    GroupName = request.groupDto.GroupName,
                    Description = request.groupDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedByUserId = userId,
                };

                await groupRepository.Create(group);

                // Thêm record vào UserGroup để chủ nhóm là thành viên mặc định
                var userGroup = new UserGroup
                {
                    GroupId = group.GroupId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                };
                await userGroupRepository.Create(userGroup);

                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Tạo nhóm thành công";
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
