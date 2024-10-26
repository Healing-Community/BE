using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class ExpertAvailabilityRepository(ExpertDbContext expertDbContext) : IExpertAvailabilityRepository
    {
        public async Task Create(ExpertAvailability entity)
        {
            await expertDbContext.ExpertAvailabilities.AddAsync(entity);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var availability = await expertDbContext.ExpertAvailabilities.FindAsync(id);
            if (availability != null)
            {
                expertDbContext.ExpertAvailabilities.Remove(availability);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<ExpertAvailability?> GetByIdAsync(string id)
        {
            return await expertDbContext.ExpertAvailabilities.FindAsync(id);
        }

        public async Task<ExpertAvailability?> GetByPropertyAsync(Expression<Func<ExpertAvailability, bool>> predicate)
        {
            return await expertDbContext.ExpertAvailabilities
                                         .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<ExpertAvailability>> GetsAsync()
        {
            return await expertDbContext.ExpertAvailabilities.ToListAsync();
        }

        public async Task Update(string id, ExpertAvailability entity)
        {
            var existingAvailability = await expertDbContext.ExpertAvailabilities.FindAsync(id);
            if (existingAvailability != null)
            {
                expertDbContext.Entry(existingAvailability).CurrentValues.SetValues(entity);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<ExpertAvailability?> GetByDateAndTimeAsync(string expertProfileId, DateTime availableDate, TimeSpan startTime, TimeSpan endTime)
        {
            return await expertDbContext.ExpertAvailabilities
                .FirstOrDefaultAsync(ea =>
                    ea.ExpertProfileId == expertProfileId &&
                    ea.AvailableDate.Date == availableDate.Date &&
                    ea.StartTime == startTime &&
                    ea.EndTime == endTime);
        }

        public async Task<IEnumerable<ExpertAvailability>> GetByExpertProfileIdAsync(string expertProfileId)
        {
            return await expertDbContext.ExpertAvailabilities
                .Where(ea => ea.ExpertProfileId == expertProfileId && ea.Status == 0)
                .ToListAsync();
        }
    }
}
