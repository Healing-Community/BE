using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.UserGroups.GetUserGroupsByGroupId
{
    public class GetUserGroupsByGroupIdQueryHandler : IRequestHandler<GetUserGroupsByGroupIdQuery, BaseResponse<List<UserGroupByGroupIdDto>>>
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public GetUserGroupsByGroupIdQueryHandler(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<List<UserGroupByGroupIdDto>>> Handle(GetUserGroupsByGroupIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<UserGroupByGroupIdDto>>()
            {
                Id = request.GroupId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userGroups = await _userGroupRepository.GetUserGroupsByGroupIdAsync(request.GroupId);
                if (userGroups == null || userGroups.Count == 0)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy User nào trong Group ID: {request.GroupId}.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = userGroups;
                response.StatusCode = 200;
                response.Message = "Lấy ra danh sách User trong Group thành công.";
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách User trong Group.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }

            return response;
        }
    }
}
