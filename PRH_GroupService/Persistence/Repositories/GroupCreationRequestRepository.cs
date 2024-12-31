using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class GroupCreationRequestRepository(HFDBGroupServiceContext hFDBGroupServiceContext) : IGroupCreationRequestRepository
    {
        public async Task Create(GroupCreationRequest entity)
        {
            await hFDBGroupServiceContext.GroupCreationRequests.AddAsync(entity);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                hFDBGroupServiceContext.GroupCreationRequests.Remove(entity);
                await hFDBGroupServiceContext.SaveChangesAsync();
            }
        }

        public async Task<GroupCreationRequest> GetByIdAsync(string id)
        {
            return await hFDBGroupServiceContext.GroupCreationRequests.FirstAsync(x => x.GroupRequestId == id);
        }

        public async Task<GroupCreationRequest> GetByPropertyAsync(Expression<Func<GroupCreationRequest, bool>> predicate)
        {
            return await hFDBGroupServiceContext.GroupCreationRequests.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new GroupCreationRequest()
            {
                GroupRequestId = Ulid.Empty.ToString()
            };
        }

        public async Task<IEnumerable<GroupCreationRequest>> GetByPropertyListAsync(Expression<Func<GroupCreationRequest, bool>> predicate)
        {
            return await hFDBGroupServiceContext.GroupCreationRequests.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<GroupCreationRequest>> GetsAsync()
        {
            return await hFDBGroupServiceContext.GroupCreationRequests.ToListAsync();
        }

        public async Task Update(string id, GroupCreationRequest entity)
        {
            var existingRequest = await hFDBGroupServiceContext.GroupCreationRequests.FirstOrDefaultAsync(x => x.GroupRequestId == id);
            if (existingRequest == null) return;
            hFDBGroupServiceContext.Entry(existingRequest).CurrentValues.SetValues(entity);
            hFDBGroupServiceContext.Entry(existingRequest).State = EntityState.Modified;
            await hFDBGroupServiceContext.SaveChangesAsync();
        }
    }
}
