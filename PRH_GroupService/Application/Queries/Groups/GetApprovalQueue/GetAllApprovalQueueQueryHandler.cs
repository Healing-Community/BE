using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;
using Application.Commons.Tools;
using Domain.Enum;

namespace Application.Queries.Groups.GetApprovalQueue
{
    public class GetAllApprovalQueueQueryHandler : IRequestHandler<GetAllApprovalQueueQuery, BaseResponse<IEnumerable<ApprovalQueueDto>>>
    {
        private readonly IApprovalQueueRepository _approvalQueueRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;

        public GetAllApprovalQueueQueryHandler(
            IApprovalQueueRepository approvalQueueRepository,
            IUserGroupRepository userGroupRepository,
            IGroupRepository groupRepository)
        {
            _approvalQueueRepository = approvalQueueRepository;
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
        }

        public async Task<BaseResponse<IEnumerable<ApprovalQueueDto>>> Handle(GetAllApprovalQueueQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ApprovalQueueDto>>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId của người thực hiện từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền truy cập.";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra xem nhóm có tồn tại không
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy nhóm với ID: {request.GroupId}.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Kiểm tra quyền của người dùng trong nhóm (Owner hoặc Moderator)
                if (group.CreatedByUserId != userId)
                {
                    var userGroup = await _userGroupRepository.GetByGroupAndUserIdAsync(request.GroupId, userId);
                    if (userGroup == null 
                        || (userGroup.RoleInGroup != RoleInGroup.Owner.ToString() 
                        && userGroup.RoleInGroup != RoleInGroup.Moderator.ToString()))
                    {
                        response.Success = false;
                        response.Message = "Bạn không có quyền xem danh sách phê duyệt.";
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return response;
                    }
                }

                // Lấy danh sách hàng chờ phê duyệt
                var approvalQueue = await _approvalQueueRepository.GetAllByGroupIdAsync(request.GroupId);

                if (approvalQueue == null || !approvalQueue.Any())
                {
                    response.Success = false;
                    response.Message = "Không có thành viên nào trong hàng chờ phê duyệt.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                response.Data = approvalQueue.Select(x => new ApprovalQueueDto
                {
                    QueueId = x.QueueId,
                    GroupId = x.GroupId,
                    UserId = x.UserId,
                    RequestedAt = x.RequestedAt
                });

                response.Success = true;
                response.Message = "Lấy danh sách hàng chờ phê duyệt thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Lỗi khi lấy danh sách hàng chờ phê duyệt.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }

}
