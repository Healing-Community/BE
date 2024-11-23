using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class GroupRepository(HFDBGroupServiceContext hFDBGroupServiceContext) : IGroupRepository
    {
        public async Task Create(Group entity)
        {
            await hFDBGroupServiceContext.Groups.AddAsync(entity);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var group = await hFDBGroupServiceContext.Groups.FirstOrDefaultAsync(x => x.GroupId == id);
            if (group == null) return;
            hFDBGroupServiceContext.Groups.Remove(group);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task<Group> GetByIdAsync(string id)
        {
            return await hFDBGroupServiceContext.Groups.FirstAsync(x => x.GroupId == id);
        }

        public async Task<Group> GetByPropertyAsync(Expression<Func<Group, bool>> predicate)
        {
            return await hFDBGroupServiceContext.Groups.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Group() { GroupId = Ulid.Empty.ToString() };
        }

        public async Task<IEnumerable<Group>> GetPublicGroupsAsync()
        {
            return await hFDBGroupServiceContext.Groups.Where(g => g.GroupVisibility == 0).ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetsAsync()
        {
            return await hFDBGroupServiceContext.Groups.ToListAsync();
        }

        public async Task Update(string id, Group entity)
        {
            var existingGroup = await hFDBGroupServiceContext.Groups.FirstOrDefaultAsync(x => x.GroupId == id);
            if (existingGroup == null) return;
            hFDBGroupServiceContext.Entry(existingGroup).CurrentValues.SetValues(entity);
            hFDBGroupServiceContext.Entry(existingGroup).State = EntityState.Modified;
            await hFDBGroupServiceContext.SaveChangesAsync();
        }
    }
}
