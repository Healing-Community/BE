using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.Groups.GetGroupsById
{
    public class GetGroupsByIdQueryHandler(IGroupRepository groupRepository) : IRequestHandler<GetGroupsByIdQuery, BaseResponse<Group>>
    {
        public async Task<BaseResponse<Group>> Handle(GetGroupsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Group>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>() 
            };

            try
            {
                var group = await groupRepository.GetByIdAsync(request.Id);
                if (group == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy thông tin có ID: {request.Id}.";
                    response.StatusCode = 404;
                    return response;
                }
                response.Success = true;
                response.Data = group;
                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy lịch trống.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }
            return response;
        }
    }
}
