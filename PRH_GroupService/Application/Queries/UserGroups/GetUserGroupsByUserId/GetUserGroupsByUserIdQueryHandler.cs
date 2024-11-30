using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;


namespace Application.Queries.UserGroups.GetUserGroupsByUserId
{
    public class GetUserGroupsByUserIdQueryHandler : IRequestHandler<GetUserGroupsByUserIdQuery, BaseResponse<List<UserGroupByUserIdDto>>>
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public GetUserGroupsByUserIdQueryHandler(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public async Task<BaseResponse<List<UserGroupByUserIdDto>>> Handle(GetUserGroupsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<UserGroupByUserIdDto>>()
            {
                Id = request.UserId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userGroups = await _userGroupRepository.GetUserGroupsByUserIdAsync(request.UserId);
                if (userGroups == null || userGroups.Count == 0)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy Group nào có User ID: {request.UserId}.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = userGroups;
                response.StatusCode = 200;
                response.Message = "Lấy ra danh sách Group của User thành công.";
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách Group của User.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }

            return response;
        }
    }
}
