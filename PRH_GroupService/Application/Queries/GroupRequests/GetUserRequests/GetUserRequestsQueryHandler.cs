using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GroupRequests.GetUserRequests
{
    public class GetUserRequestsQueryHandler : IRequestHandler<GetUserRequestsQuery, BaseResponse<IEnumerable<GroupRequestDto>>>
    {
        private readonly IGroupCreationRequestRepository _requestRepository;

        public GetUserRequestsQueryHandler(IGroupCreationRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<BaseResponse<IEnumerable<GroupRequestDto>>> Handle(GetUserRequestsQuery request, CancellationToken cancellationToken)
        {
            var userRequests = await _requestRepository.GetByPropertyListAsync(r => r.RequestedById == request.UserId);

            if (!userRequests.Any())
                return BaseResponse<IEnumerable<GroupRequestDto>>.NotFound("Không tìm thấy yêu cầu nào cho người dùng này.");

            var mappedRequests = userRequests.Select(r => new GroupRequestDto
            {
                GroupRequestId = r.GroupRequestId,
                GroupName = r.GroupName,
                Description = r.Description,
                IsApproved = r.IsApproved,
                ApprovedAt = r.ApprovedAt,
                ApprovedById = r.ApprovedById,
                RequestedAt = r.RequestedAt,
                RequestedById = r.RequestedById,
            });

            return BaseResponse<IEnumerable<GroupRequestDto>>.SuccessReturn(mappedRequests, "Danh sách yêu cầu của người dùng đã được lấy thành công.");
        }
    }
}
