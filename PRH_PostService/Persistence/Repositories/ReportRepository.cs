using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class ReportRepository(HFDBPostserviceContext hFDBPostserviceContext) : IReportRepository
    {
        public async Task Create(Report entity)
        {
            await hFDBPostserviceContext.Reports.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var report = await hFDBPostserviceContext.Reports.FirstOrDefaultAsync(x => x.ReportId == id);
            if (report == null) return;
            hFDBPostserviceContext.Reports.Remove(report);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Report> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.Reports.FirstAsync(x => x.ReportId == id);
        }

        public async Task<Report> GetByPropertyAsync(Expression<Func<Report, bool>> predicate)
        {
            var report = await hFDBPostserviceContext.Reports.AsNoTracking().FirstOrDefaultAsync(predicate);
            return report ?? new Report() { 
                ReportId = Ulid.Empty.ToString() 
            };
        }

        public async Task<IEnumerable<Report>> GetsAsync()
        {
            return await hFDBPostserviceContext.Reports.ToListAsync();
        }

        public Task<IEnumerable<Report>?> GetsByPropertyAsync(Expression<Func<Report, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, Report entity)
        {
            var existingReport = await hFDBPostserviceContext.Reports.FirstOrDefaultAsync(x => x.ReportId == id);
            if (existingReport == null) return;
            hFDBPostserviceContext.Entry(existingReport).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReport).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
