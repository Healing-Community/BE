using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;

namespace Application.Queries.Roles.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, BaseResponse<IEnumerable<Role>>>
{
    public async Task<BaseResponse<IEnumerable<Role>>> Handle(GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseResponse<IEnumerable<Role>>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        try
        {
            // Lấy danh sách vai trò từ repository
            var roles = await roleRepository.GetsAsync();

            // Kiểm tra nếu không tìm thấy vai trò nào
            if (roles == null || !roles.Any())
            {
                response.Message = "Không tìm thấy vai trò nào.";
                response.Success = false;
                response.StatusCode = 404; // Trả về lỗi 404 (Not Found)
                response.Data = null;
            }
            else
            {
                // Nếu tìm thấy vai trò
                response.Message = "Lấy danh sách vai trò thành công.";
                response.Success = true;
                response.StatusCode = 200; // Trả về 200 (OK)
                response.Data = roles;
            }
        }
        catch (Exception e)
        {
            // Xử lý khi có lỗi xảy ra
            response.Message = "Đã xảy ra lỗi khi lấy danh sách vai trò: " + e.Message;
            response.Success = false;
            response.StatusCode = 500; // Trả về 500 (Internal Server Error)
        }

        return response;
    }
}
