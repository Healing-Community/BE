using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ApprovalQueueRepository(HFDBGroupServiceContext context) : IApprovalQueueRepository
    {
        public async Task Create(ApprovalQueue entity)
        {
            await context.ApprovalQueues.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.ApprovalQueues.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ApprovalQueue>> GetAllByGroupIdAsync(string groupId)
        {
            return await context.ApprovalQueues.Where(aq => aq.GroupId == groupId).ToListAsync();
        }

        public async Task<ApprovalQueue?> GetByIdAsync(string id)
        {
            return await context.ApprovalQueues.FirstOrDefaultAsync(a => a.QueueId == id);
        }

        public Task<ApprovalQueue> GetByPropertyAsync(Expression<Func<ApprovalQueue, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApprovalQueue>> GetPendingApprovalsByGroupIdAsync(string groupId)
        {
            return await context.ApprovalQueues
                    .Where(a => a.GroupId == groupId && !a.IsApproved)
                    .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalQueue>> GetsAsync()
        {
            return await context.ApprovalQueues.ToListAsync();
        }

        public Task Update(string id, ApprovalQueue entity)
        {
            throw new NotImplementedException();
        }
    }
}
