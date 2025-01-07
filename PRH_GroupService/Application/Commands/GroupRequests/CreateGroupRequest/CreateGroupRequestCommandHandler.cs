using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System.Security.Claims;


namespace Application.Commands.GroupRequests.CreateGroupRequest
{
    public class CreateGroupRequestCommandHandler : IRequestHandler<CreateGroupRequestCommand, BaseResponse<string>>
    {
        private readonly IGroupCreationRequestRepository _requestRepository;
        private readonly IHttpContextAccessor _accessor;

        public CreateGroupRequestCommandHandler(IGroupCreationRequestRepository requestRepository, IHttpContextAccessor accessor)
        {
            _requestRepository = requestRepository;
            _accessor = accessor;
        }

        public async Task<BaseResponse<string>> Handle(CreateGroupRequestCommand request, CancellationToken cancellationToken)
        {
            var userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);


            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role) || (role != "User" && role != "Expert"))
                return BaseResponse<string>.Unauthorized();

            // Kiểm tra nếu người dùng đã có yêu cầu đang chờ phê duyệt
            var existingRequest = await _requestRepository.GetByPropertyAsync(r => r.RequestedById == userId && r.IsApproved == null);
            if (existingRequest.GroupRequestId != Ulid.Empty.ToString())
            {
                return BaseResponse<string>.BadRequest("Bạn đã gửi một yêu cầu và đang chờ phê duyệt.");
            }

            var newRequest = new GroupCreationRequest
            {
                GroupRequestId = Ulid.NewUlid().ToString(),
                RequestedById = userId,
                GroupName = request.GroupName,
                Description = request.Description,
                CoverImg = request.CoverImg,
                RequestedAt = DateTime.UtcNow.AddHours(7)
            };

            await _requestRepository.Create(newRequest);
            return BaseResponse<string>.SuccessReturn("Yêu cầu tạo nhóm đã được gửi.");
        }
    }
}
