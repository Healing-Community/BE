using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;


namespace Application.Queries.UserGroups.GetRoleInGroupByGroupIdAndUserId
{
    public class GetRoleInGroupQueryHandler : IRequestHandler<GetRoleInGroupQuery, BaseResponse<RoleInGroupDto>>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;

        public GetRoleInGroupQueryHandler(IUserGroupRepository userGroupRepository, IGroupRepository groupRepository)
        {
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<RoleInGroupDto>> Handle(GetRoleInGroupQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<RoleInGroupDto>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Nhóm với GroupId '{request.GroupId}' không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.UserId);
                if (userGroup == null)
                {
                    response.Success = false;
                    response.Message = $"Người dùng với UserId '{request.UserId}' không tồn tại trong nhóm.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                var roleInGroupDto = new RoleInGroupDto
                {
                    RoleInGroup = userGroup.RoleInGroup
                };

                response.Data = roleInGroupDto;
                response.Success = true;
                response.Message = "Lấy Role In Group thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi khi xử lý yêu cầu.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
