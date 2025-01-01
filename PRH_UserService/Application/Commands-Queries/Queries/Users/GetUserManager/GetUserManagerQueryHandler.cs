using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Queries.Users.GetUserManager;

public class GetUserManagerQueryHandler(IHttpContextAccessor accessor, IUserRepository userRepository, IRoleRepository roleRepository) : IRequestHandler<GetUserManagerQuery, BaseResponse<IEnumerable<UserDto>>>
{
    public async Task<BaseResponse<IEnumerable<UserDto>>> Handle(GetUserManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Extract user info from HttpContext
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<IEnumerable<UserDto>>.Unauthorized();

            var userRole = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            var userRoleInDb = await roleRepository.GetByPropertyAsync(x => x.RoleName == userRole);
            var userRoleInDbId = userRoleInDb?.RoleId;

            if (userRoleInDbId == null) return BaseResponse<IEnumerable<UserDto>>.Unauthorized();

            // Determine target roles based on current user role
            IEnumerable<int> targetRoleIds = userRoleInDbId switch
            {
                3 => [1, 4], // Moderator can view users and experts
                2 => [3]    // Admin can view moderators
,
                _ => throw new NotImplementedException()
            };

            if (targetRoleIds == null) return BaseResponse<IEnumerable<UserDto>>.BadRequest();

            // Fetch and map users
            var users = await userRepository.GetsByPropertyAsync(x => targetRoleIds.Contains(x.RoleId));
            if (users == null) return BaseResponse<IEnumerable<UserDto>>.NotFound();
            var userDtos = await MapUsersToUserDtos(users);

            return BaseResponse<IEnumerable<UserDto>>.SuccessReturn(userDtos);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<UserDto>>.InternalServerError(message: e.Message);
        }
    }

    // Helper method to map users to UserDto
    private async Task<IEnumerable<UserDto>> MapUsersToUserDtos(IEnumerable<User> users)
    {
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var role = await roleRepository.GetByIdAsync(user.RoleId.ToString());
            userDtos.Add(new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Role = role?.RoleName ?? string.Empty,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }
        return userDtos;
    }
}
