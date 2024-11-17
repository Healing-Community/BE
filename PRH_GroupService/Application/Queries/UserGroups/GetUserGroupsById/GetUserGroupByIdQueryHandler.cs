using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.UserGroups.GetUserGroupsById
{
    public class GetUserGroupByIdQueryHandler : IRequestHandler<GetUserGroupByIdQuery, BaseResponse<UserGroup>>
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public GetUserGroupByIdQueryHandler(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<UserGroup>> Handle(GetUserGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<UserGroup>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var userGroup = await _userGroupRepository.GetByIdInGroupAsync(request.GroupId, request.UserId);

                if (userGroup == null)
                {
                    response.StatusCode = 404;
                    response.Message = $"Không tìm thấy UserGroup với GroupId: {request.GroupId} và UserId: {request.UserId}.";
                    response.Success = false;
                    return response;
                }

                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công.";
                response.Success = true;
                response.Data = userGroup;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin UserGroup.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }

            return response;
        }
    }
}
