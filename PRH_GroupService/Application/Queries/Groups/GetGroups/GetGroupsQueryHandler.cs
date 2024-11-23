using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.Groups.GetGroups
{
    public class GetGroupsQueryHandler(IGroupRepository groupRepository) 
        : IRequestHandler<GetGroupsQuery, BaseResponse<IEnumerable<Group>>>
    {
        public async Task<BaseResponse<IEnumerable<Group>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Group>>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var groups = await groupRepository.GetsAsync();

                if (!groups.Any())
                {
                    response.StatusCode = 404;
                    response.Message = "Không có nhóm nào được tìm thấy.";
                    response.Success = false;
                    response.Data = Enumerable.Empty<Group>(); // Trả về danh sách rỗng thay vì null
                    return response;
                }

                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = groups;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Có lỗi xày ra khi xem danh sách nhóm.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {e.Message}");
            }
            return response;
        }
    }
}
