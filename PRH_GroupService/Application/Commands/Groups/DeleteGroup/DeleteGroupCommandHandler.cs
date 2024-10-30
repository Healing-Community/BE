using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.Groups.DeleteGroup
{
    public class DeleteGroupCommandHandler(IGroupRepository groupRepository) : IRequestHandler<DeleteGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var existingGroup = await groupRepository.GetByIdAsync(request.Id);
                if (existingGroup == null)
                {
                    response.StatusCode = 404;
                    response.Success = false;
                    response.Message = "Nhóm không tồn tại.";
                    return response;
                }

                await groupRepository.DeleteAsync(request.Id);
                response.StatusCode = 200;
                response.Success = true;
                response.Message = "Xoá nhóm thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Lỗi !!! Xoá nhóm thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
