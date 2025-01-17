using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
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

        public async Task DeleteAsync(string id)
        {
            var reaction = await hFDBPostserviceContext.Reactions.FirstOrDefaultAsync(x => x.ReactionId == id);
            if (reaction == null) return;
            hFDBPostserviceContext.Reactions.Remove(reaction);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Reaction> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.Reactions.FirstAsync(x => x.ReactionId == id);
        }

        public async Task<Reaction?> GetByPropertyAsync(Expression<Func<Reaction, bool>> predicate)
        {
            return await hFDBPostserviceContext.Reactions.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Reaction>> GetsAsync()
        {
            return await hFDBPostserviceContext.Reactions.ToListAsync();
        }

        public async Task Update(string id, Reaction entity)
        {
            var existingReaction = await hFDBPostserviceContext.Reactions.FirstOrDefaultAsync(x => x.ReactionId == id);
            if (existingReaction == null) return;
            hFDBPostserviceContext.Entry(existingReaction).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReaction).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Reaction>?> GetsByPropertyAsync(Expression<Func<Reaction, bool>> predicate , int size = int.MaxValue)
        {
            return await hFDBPostserviceContext.Reactions.AsNoTracking().Where(predicate).Take(size).ToListAsync();
        }

        public async Task<IEnumerable<Reaction>> GetsMostReactedPost(int top)
        {
            // Get top 5 most reacted posts base on the number of postId
            var reaction = await hFDBPostserviceContext.Reactions.GroupBy(x => x.PostId)
                .Select(x => new { PostId = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(top)
                .ToListAsync();
            
            var reactions = new List<Reaction>();
            foreach (var react in reaction)
            {
                var reactionEntity = await hFDBPostserviceContext.Reactions.FirstAsync(x => x.PostId == react.PostId);
                if (reactionEntity == null) continue;
                reactions.Add(reactionEntity);
            }
            return reactions;
        }

        public async Task<Reaction?> GetReactionByShareIdAsync(string shareId, string userId)
        {
            return await hFDBPostserviceContext.Reactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ShareId == shareId && x.UserId == userId);
        }
    }
}
