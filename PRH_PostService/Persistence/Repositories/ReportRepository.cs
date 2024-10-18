using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ReportRepository(HFDBPostserviceContext hFDBPostserviceContext) : IReportRepository
    {
        public async Task Create(Report entity)
        {
            await hFDBPostserviceContext.Reports.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var report = await hFDBPostserviceContext.Reports.FirstOrDefaultAsync(x => x.ReportId == id);
            if (report == null) return;
            hFDBPostserviceContext.Reports.Remove(report);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<Report> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.Reports.FirstAsync(x => x.ReportId == id);
        }

        public async Task<Report> GetByPropertyAsync(Expression<Func<Report, bool>> predicate)
        {
            var report = await hFDBPostserviceContext.Reports.AsNoTracking().FirstOrDefaultAsync(predicate);
            return report ?? new Report();
        }

        public async Task<IEnumerable<Report>> GetsAsync()
        {
            return await hFDBPostserviceContext.Reports.ToListAsync();
        }

        public async Task Update(Guid id, Report entity)
        {
            var existingReport = await hFDBPostserviceContext.Reports.FirstOrDefaultAsync(x => x.ReportId == id);
            if (existingReport == null) return;
            hFDBPostserviceContext.Entry(existingReport).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReport).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
