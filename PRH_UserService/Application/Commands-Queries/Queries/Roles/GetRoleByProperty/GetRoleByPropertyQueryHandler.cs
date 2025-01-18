using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

public class GetRoleByPropertyQueryHandler(IRoleRepository roleRepository, IUserRepository userRepository) : IRequestHandler<GetRoleByPropertyQuery, BaseResponse<Role>>
{
    public async Task<BaseResponse<Role>> Handle(GetRoleByPropertyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByPropertyAsync(u=>u.UserId == request.Property);

            if (user == null)
            {
                return BaseResponse<Role>.SuccessReturn(null,message:"User not found");
            }

            var role = await roleRepository.GetByPropertyAsync(r => r.RoleId == user.RoleId);

            if (role == null)
            {
                return BaseResponse<Role>.SuccessReturn(null,message:"Role not found");
            }

            return BaseResponse<Role>.SuccessReturn(role);
        }
        catch (Exception e)
        {  
            return BaseResponse<Role>.InternalServerError(e.Message);
        }
    }
}