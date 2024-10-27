using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ExpertProfileRepository(ExpertDbContext expertDbContext) : IExpertProfileRepository
    {
        public async Task Create(ExpertProfile entity)
        {
            await expertDbContext.ExpertProfiles.AddAsync(entity);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var expertProfile = await expertDbContext.ExpertProfiles.FindAsync(id);
            if (expertProfile != null)
            {
                expertDbContext.ExpertProfiles.Remove(expertProfile);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<ExpertProfile?> GetByIdAsync(string id)
        {
            return await expertDbContext.ExpertProfiles.FindAsync(id);
        }

        public async Task<ExpertProfile?> GetByPropertyAsync(Expression<Func<ExpertProfile, bool>> predicate)
        {
            return await expertDbContext.ExpertProfiles
                                         .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ExpertProfile>> GetsAsync()
        {
            return await expertDbContext.ExpertProfiles.ToListAsync();
        }

        public async Task Update(string id, ExpertProfile entity)
        {
            var existingExpertProfile = await expertDbContext.ExpertProfiles.FindAsync(id);
            if (existingExpertProfile != null)
            {
                expertDbContext.Entry(existingExpertProfile).CurrentValues.SetValues(entity);
                await expertDbContext.SaveChangesAsync();
            }
        }
    }
}
