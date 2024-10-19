using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
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

        public async Task DeleteAsync(Guid id)
        {
            var reactionType = await hFDBPostserviceContext.ReactionTypes.FirstOrDefaultAsync(x => x.ReactionTypeId == id);
            if (reactionType == null) return;
            hFDBPostserviceContext.ReactionTypes.Remove(reactionType);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<ReactionType> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.ReactionTypes.FirstAsync(x => x.ReactionTypeId == id);
        }

        public async Task<ReactionType> GetByPropertyAsync(Expression<Func<ReactionType, bool>> predicate)
        {
            return await hFDBPostserviceContext.ReactionTypes.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new ReactionType();
        }

        public async Task<IEnumerable<ReactionType>> GetsAsync()
        {
            return await hFDBPostserviceContext.ReactionTypes.ToListAsync();
        }

        public async Task Update(Guid id, ReactionType entity)
        {
            var existingReactionType = await hFDBPostserviceContext.ReactionTypes.FirstOrDefaultAsync(x => x.ReactionTypeId == id);
            if (existingReactionType == null) return;
            hFDBPostserviceContext.Entry(existingReactionType).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReactionType).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
