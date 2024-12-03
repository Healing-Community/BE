using Application.Commons.DTOs;
using Application.Interfaces.GenericRepository;
using Domain.Entities;
using Domain.Enum;

namespace Application.Interfaces.Repository
{
    public interface IUserGroupRepository : IReadRepository<UserGroup>, ICreateRepository<UserGroup>, IUpdateRepository<UserGroup>, IDeleteRepository
    {
        Task<UserGroup?> GetByGroupAndUserIdAsync(string groupId, string userId);
        Task<UserGroup?> GetByIdInGroupAsync(string groupId, string userId);
        Task DeleteAsyncV2(string groupId, string userId);
        Task UpdateRole(string groupId, string userId, RoleInGroup role);
        Task<List<UserGroupByUserIdDto>> GetUserGroupsByUserIdAsync(string userId);
        Task<List<UserGroupByGroupIdDto>> GetUserGroupsByGroupIdAsync(string groupId);

    }
}
