using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class ReportTypeRepository(HFDBPostserviceContext hFDBPostserviceContext) : IReportTypeRepository
    {
        public async Task Create(ReportType entity)
        {
            await hFDBPostserviceContext.ReportTypes.AddAsync(entity);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var reportType = await hFDBPostserviceContext.ReportTypes.FirstOrDefaultAsync(x => x.ReportTypeId == id);
            if (reportType == null) return;
            hFDBPostserviceContext.ReportTypes.Remove(reportType);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<ReportType> GetByIdAsync(Guid id)
        {
            return await hFDBPostserviceContext.ReportTypes.FirstAsync(x => x.ReportTypeId == id);
        }

        public async Task<ReportType> GetByPropertyAsync(Expression<Func<ReportType, bool>> predicate)
        {
            var reportType = await hFDBPostserviceContext.ReportTypes.AsNoTracking().FirstOrDefaultAsync(predicate);
            return reportType ?? new ReportType();
        }

        public async Task<IEnumerable<ReportType>> GetsAsync()
        {
            return await hFDBPostserviceContext.ReportTypes.ToListAsync();
        }

        public async Task Update(Guid id, ReportType entity)
        {
            var existingReportType = await hFDBPostserviceContext.ReportTypes.FirstOrDefaultAsync(x => x.ReportTypeId == id);
            if (existingReportType == null) return;
            hFDBPostserviceContext.Entry(existingReportType).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReportType).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
