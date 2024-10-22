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
            var expertProfiles = await expertDbContext.ExpertProfiles.FirstOrDefaultAsync(x => x.ExpertProfileId == id);
            if (expertProfiles == null) return;
            expertDbContext.ExpertProfiles.Remove(expertProfiles);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task<ExpertProfile> GetByIdAsync(string id)
        {
            return await expertDbContext.ExpertProfiles.FirstAsync(x => x.ExpertProfileId == id);
        }

        public async Task<ExpertProfile> GetByPropertyAsync(Expression<Func<ExpertProfile, bool>> predicate)
        {
            return await expertDbContext.ExpertProfiles.AsNoTracking().FirstOrDefaultAsync(predicate)
                               ?? throw new InvalidOperationException("Expert Profiles not found");
        }

        public async Task<IEnumerable<ExpertProfile>> GetsAsync()
        {
            return await expertDbContext.ExpertProfiles.ToListAsync();
        }

        public async Task Update(string id, ExpertProfile entity)
        {
            var existingExpertProfile = await expertDbContext.ExpertProfiles.FirstOrDefaultAsync(x => x.ExpertProfileId == id);
            if (existingExpertProfile == null) return;
            expertDbContext.Entry(existingExpertProfile).CurrentValues.SetValues(entity);
            expertDbContext.Entry(existingExpertProfile).State = EntityState.Modified;
            await expertDbContext.SaveChangesAsync();
        }
    }
}
