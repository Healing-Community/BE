using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Commands.Groups.DeleteGroup
{
    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<string>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteGroupCommandHandler(IGroupRepository groupRepository, IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseResponse<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId của người thực hiện từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(_httpContextAccessor.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra xem nhóm có tồn tại không
                var group = await _groupRepository.GetByIdAsync(request.Id);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy nhóm với ID: {request.Id}.";
                    response.StatusCode = 404;
                    return response;
                }

                // Kiểm tra nếu người thực hiện không phải là chủ nhóm
                if (group.CreatedByUserId != userId)
                {
                    response.Success = false;
                    response.Message = "Chỉ có chủ nhóm mới có quyền xóa nhóm.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                // Xóa nhóm
                await _groupRepository.DeleteAsync(request.Id);
                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Xóa nhóm thành công.";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Xóa nhóm thất bại.";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
