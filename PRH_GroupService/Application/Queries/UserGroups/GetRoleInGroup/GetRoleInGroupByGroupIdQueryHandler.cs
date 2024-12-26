using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;


namespace Application.Queries.UserGroups.GetRoleInGroup
{
    public class GetRoleInGroupByGroupIdQueryHandler : IRequestHandler<GetRoleInGroupByGroupIdQuery, BaseResponse<RoleCountDto>>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;

        public GetRoleInGroupByGroupIdQueryHandler(IUserGroupRepository userGroupRepository, IGroupRepository groupRepository)
        {
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<RoleCountDto>> Handle(GetRoleInGroupByGroupIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<RoleCountDto>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Check if the group exists
                var groupExists = await _groupRepository.ExistsAsync(request.GroupId);
                if (!groupExists)
                {
                    response.Success = false;
                    response.Message = $"Nhóm với GroupId '{request.GroupId}' không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Count roles in the group
                var roleCounts = await _userGroupRepository.CountRolesByGroupIdAsync(request.GroupId);

                response.Success = true;
                response.Message = "Lấy số lượng thành viên trong nhóm theo vai trò thành công.";
                response.Data = roleCounts;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi khi lấy số lượng thành viên.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
    }
}
