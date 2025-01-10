using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.GroupRequests.GetApprovalRequests
{
    public class GetApprovalRequestsQueryHandler : IRequestHandler<GetApprovalRequestsQuery, BaseResponse<IEnumerable<GroupRequestDto>>>
    {
        private readonly IGroupCreationRequestRepository _requestRepository;

        public GetApprovalRequestsQueryHandler(IGroupCreationRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<BaseResponse<IEnumerable<GroupRequestDto>>> Handle(GetApprovalRequestsQuery request, CancellationToken cancellationToken)
        {
            // Chỉ `Admin` hoặc `Moderator` mới có quyền xem danh sách yêu cầu phê duyệt
            if (request.UserRole != "Admin" && request.UserRole != "Moderator")
                return BaseResponse<IEnumerable<GroupRequestDto>>.Unauthorized();

            var approvalRequests = await _requestRepository.GetByPropertyListAsync(r => r.IsApproved == null);

            if (!approvalRequests.Any())
                return BaseResponse<IEnumerable<GroupRequestDto>>.NotFound("Không có yêu cầu nào cần phê duyệt.");

            var mappedRequests = approvalRequests.Select(r => new GroupRequestDto
            {
                GroupRequestId = r.GroupRequestId,
                GroupName = r.GroupName,
                Description = r.Description,
                CoverImg = r.CoverImg,
                RequestedById = r.RequestedById,
                RequestedAt = r.RequestedAt
            });

            return BaseResponse<IEnumerable<GroupRequestDto>>.SuccessReturn(mappedRequests, "Danh sách yêu cầu cần phê duyệt đã được lấy thành công.");
        }
    }
}
