using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class WorkExperienceRepository(ExpertDbContext expertDbContext) : IWorkExperienceRepository
    {
        public async Task Create(WorkExperience entity)
        {
            await expertDbContext.WorkExperiences.AddAsync(entity);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var workExperience = await expertDbContext.WorkExperiences.FindAsync(id);
            if (workExperience != null)
            {
                expertDbContext.WorkExperiences.Remove(workExperience);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<WorkExperience?> GetByIdAsync(string id)
        {
            return await expertDbContext.WorkExperiences.FindAsync(id);
        }

        public async Task<WorkExperience?> GetByPropertyAsync(Expression<Func<WorkExperience, bool>> predicate)
        {
            return await expertDbContext.WorkExperiences
                                         .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<WorkExperience>> GetsAsync()
        {
            return await expertDbContext.WorkExperiences.ToListAsync();
        }

        public async Task Update(string id, WorkExperience entity)
        {
            var existingExperience = await expertDbContext.WorkExperiences.FindAsync(id);
            if (existingExperience != null)
            {
                expertDbContext.Entry(existingExperience).CurrentValues.SetValues(entity);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WorkExperience>> GetWorkExperiencesByExpertIdAsync(string expertProfileId)
        {
            return await expertDbContext.WorkExperiences
                .Where(w => w.ExpertProfileId == expertProfileId)
                .ToListAsync();
        }

    }
}
