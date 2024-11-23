using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class UserGroupRepository(HFDBGroupServiceContext hFDBGroupServiceContext) : IUserGroupRepository
    {
        public async Task Create(UserGroup entity)
        {
            await hFDBGroupServiceContext.UserGroups.AddAsync(entity);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var userGroup = await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(x => x.UserId == id);
            if (userGroup == null) return;
            hFDBGroupServiceContext.UserGroups.Remove(userGroup);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task DeleteAsyncV2(string groupId, string userId)
        {
            var userGroup = await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
            if (userGroup == null) return;
            hFDBGroupServiceContext.UserGroups.Remove(userGroup);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task<UserGroup?> GetByGroupAndUserIdAsync(string groupId, string userId)
        {
            return await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);
        }

        public async Task<UserGroup> GetByIdAsync(string id)
        {
            return await hFDBGroupServiceContext.UserGroups.FirstAsync(x => x.UserId == id);
        }

        public async Task<UserGroup?> GetByIdInGroupAsync(string groupId, string userId)
        {
            return await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
        }

        public async Task<UserGroup> GetByPropertyAsync(Expression<Func<UserGroup, bool>> predicate)
        {
            return await hFDBGroupServiceContext.UserGroups.AsNoTracking().FirstOrDefaultAsync(predicate) ?? 
                new UserGroup() { 
                    GroupId = Ulid.Empty.ToString() 
                };
        }

        public async Task<IEnumerable<UserGroup>> GetsAsync()
        {
            return await hFDBGroupServiceContext.UserGroups.ToListAsync();
        }

        public async Task Update(string id, UserGroup entity)
        {
            var existingUserGroup = await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(x => x.UserId == id);
            if (existingUserGroup == null) return;
            hFDBGroupServiceContext.Entry(existingUserGroup).CurrentValues.SetValues(entity);
            hFDBGroupServiceContext.Entry(existingUserGroup).State = EntityState.Modified;
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task UpdateRole(string groupId, string userId, RoleInGroup role)
        {
            var userGroup = await hFDBGroupServiceContext.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
            if (userGroup == null) return;
            userGroup.RoleInGroup = role.ToString();
            hFDBGroupServiceContext.Entry(userGroup).State = EntityState.Modified;
            await hFDBGroupServiceContext.SaveChangesAsync();
        }
    }
}
