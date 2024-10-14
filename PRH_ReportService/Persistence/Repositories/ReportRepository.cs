using Application.Interfaces;
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
    public class ReportRepository(ReportDbContext context) : IReportRepository
    {
        public async Task Create(Report entity)
        {
            await context.Reports.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var report = await context.Reports.FindAsync(id);
            if (report == null)
            {
                throw new Exception("Report not found");
            }
            context.Reports.Remove(report);
            await context.SaveChangesAsync();

        }

        public async Task<Report> GetByIdAsync(Guid id)
        {
            return await context.Reports.FirstAsync(r=>r.ReportId == id);
        }

        public async Task<Report> GetByPropertyAsync(Expression<Func<Report, bool>> predicate)
        {
            return await context.Reports.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Report() { ReportTypeId = string.Empty};
        }

        public async Task<IEnumerable<Report>> GetsAsync()
        {
           
            return await context.Reports.ToListAsync();
        }

        public async Task Update(Guid id, Report entity)
        {
            var existingReports = await context.Reports.FirstOrDefaultAsync(x => x.ReportId == id) ?? throw new Exception("Report not found");
            context.Entry(existingReports).CurrentValues.SetValues(entity);
            context.Entry(existingReports).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
