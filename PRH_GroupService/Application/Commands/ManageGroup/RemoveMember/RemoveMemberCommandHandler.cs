using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.ManageGroup.RemoveMember
{
    public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, BaseResponse<string>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public RemoveMemberCommandHandler(IGroupRepository groupRepository, IUserGroupRepository userGroupRepository)
        {
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }
        public async Task<BaseResponse<string>> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId của người thực hiện từ HttpContext
                var ownerUserId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (ownerUserId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra xem nhóm có tồn tại không
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy nhóm với ID: {request.GroupId}.";
                    response.StatusCode = 404;
                    return response;
                }

                // Kiểm tra nếu người thực hiện không phải là chủ nhóm
                if (group.CreatedByUserId != ownerUserId)
                {
                    response.Success = false;
                    response.Message = "Chỉ có chủ nhóm mới có quyền loại bỏ thành viên.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                // Kiểm tra nếu chủ nhóm đang cố gắng xóa chính mình
                if (request.MemberUserId == ownerUserId)
                {
                    response.Success = false;
                    response.Message = "Chủ nhóm không thể xóa chính mình khỏi nhóm.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Kiểm tra nếu thành viên không tồn tại trong nhóm
                var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, request.MemberUserId);
                if (userGroup == null)
                {
                    response.Success = false;
                    response.Message = "Người dùng không tham gia nhóm này hoặc không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Xóa thành viên khỏi nhóm
                await _userGroupRepository.DeleteAsyncV2(request.GroupId, request.MemberUserId);

                var groups = await _groupRepository.GetByIdAsync(request.GroupId);
                if (groups != null)
                {
                    groups.CurrentMemberCount--;
                    await _groupRepository.UpdateAfterLeaving(groups);
                }

                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Loại bỏ thành viên khỏi nhóm thành công.";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Không thể loại bỏ thành viên khỏi nhóm.";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
