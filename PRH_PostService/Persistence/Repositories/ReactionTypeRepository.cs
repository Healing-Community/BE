using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class ReactionTypeRepository(HFDBPostserviceContext hFDBPostserviceContext) : IReactionTypeRepository
    {
        public async Task Create(ReactionType entity)
        {
            await hFDBPostserviceContext.ReactionTypes.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var reactionType = await hFDBPostserviceContext.ReactionTypes.FirstOrDefaultAsync(x => x.ReactionTypeId == id);
            if (reactionType == null) return;
            hFDBPostserviceContext.ReactionTypes.Remove(reactionType);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<ReactionType> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.ReactionTypes.FirstAsync(x => x.ReactionTypeId == id);
        }

        public async Task<ReactionType?> GetByPropertyAsync(Expression<Func<ReactionType, bool>> predicate)
        {
            return await hFDBPostserviceContext.ReactionTypes.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ReactionType>> GetsAsync()
        {
            return await hFDBPostserviceContext.ReactionTypes.ToListAsync();
        }

        public Task<IEnumerable<ReactionType>?> GetsByPropertyAsync(Expression<Func<ReactionType, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, ReactionType entity)
        {
            var existingReactionType = await hFDBPostserviceContext.ReactionTypes.FirstOrDefaultAsync(x => x.ReactionTypeId == id);
            if (existingReactionType == null) return;
            hFDBPostserviceContext.Entry(existingReactionType).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReactionType).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
