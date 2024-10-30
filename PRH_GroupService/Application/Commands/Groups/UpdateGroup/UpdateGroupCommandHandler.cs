using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System.Net;


namespace Application.Commands.Groups.UpdateGroup
{
    public class UpdateGroupCommandHandler(IGroupRepository groupRepository, IHttpContextAccessor httpContextAccessor) 
        : IRequestHandler<UpdateGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var existingGroup = await groupRepository.GetByIdAsync(request.Id);
                if (existingGroup == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy thông tin có ID: {request.Id}.";
                    response.StatusCode = 404;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (userId == null || existingGroup.CreatedByUserId != userId)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để cập nhật nhóm này.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                existingGroup.GroupName = request.GroupDto.GroupName;
                existingGroup.Description = request.GroupDto.Description;
                existingGroup.UpdatedAt = DateTime.UtcNow;

                await groupRepository.Update(request.Id, existingGroup);
                response.Success = true;
                response.Message = "Cập nhật nhóm thành công";
                response.Data = existingGroup.GroupId;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {            
                response.Success = false;
                response.Message = "Lỗi !!! Cập nhật nhóm thất bại";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }
            return response;
        }
    }
}
