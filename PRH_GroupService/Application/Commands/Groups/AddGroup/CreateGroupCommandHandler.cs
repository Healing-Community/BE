using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Groups.AddGroup
{
    public class CreateGroupCommandHandler(IGroupRepository groupRepository) : IRequestHandler<CreateGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var userId = Authentication.GetUserIdFromHttpContext(request.httpContext);
            if (userId == null)
            {
                response.Success = false;
                response.Message = "Không có quyền để truy cập";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }
            var group = new Group
            {
                GroupId = Ulid.NewUlid().ToString(),
                GroupName = request.groupDto.GroupName,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };
            try
            {
                await groupRepository.Create(group);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo nhóm thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Tạo nhóm thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
