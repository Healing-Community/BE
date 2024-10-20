using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class ReactionRepository(HFDBPostserviceContext hFDBPostserviceContext) : IReactionRepository
    {
        public async Task Create(Reaction entity)
        {
            await hFDBPostserviceContext.Reactions.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var reaction = await hFDBPostserviceContext.Reactions.FirstOrDefaultAsync(x => x.ReactionId == id);
            if (reaction == null) return;
            hFDBPostserviceContext.Reactions.Remove(reaction);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Reaction> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.Reactions.FirstAsync(x => x.ReactionId == id);
        }

        public async Task<Reaction> GetByPropertyAsync(Expression<Func<Reaction, bool>> predicate)
        {
            return await hFDBPostserviceContext.Reactions.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Reaction();
        }

        public async Task<IEnumerable<Reaction>> GetsAsync()
        {
            return await hFDBPostserviceContext.Reactions.ToListAsync();
        }

        public async Task Update(Guid id, Reaction entity)
        {
            var existingReaction = await hFDBPostserviceContext.Reactions.FirstOrDefaultAsync(x => x.ReactionId == id);
            if (existingReaction == null) return;
            hFDBPostserviceContext.Entry(existingReaction).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReaction).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
