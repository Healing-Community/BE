using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.UserGroups.GetUserGroups
{
    public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, BaseResponse<IEnumerable<UserGroup>>>
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public GetUserGroupsQueryHandler(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<IEnumerable<UserGroup>>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<UserGroup>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userGroups = await _userGroupRepository.GetsAsync();

                if (!userGroups.Any())
                {
                    response.StatusCode = 404;
                    response.Message = "Không tìm thấy bản ghi nào trong UserGroup.";
                    response.Success = false;
                    response.Data = Enumerable.Empty<UserGroup>(); // Trả về danh sách rỗng
                    return response;
                }

                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công.";
                response.Success = true;
                response.Data = userGroups;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách UserGroup.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }

            return response;
        }
    }
}
