using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
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

        public async Task DeleteAsync(string id)
        {
            var reportType = await hFDBPostserviceContext.ReportTypes.FirstOrDefaultAsync(x => x.ReportTypeId == id);
            if (reportType == null) return;
            hFDBPostserviceContext.ReportTypes.Remove(reportType);
            await hFDBPostserviceContext.SaveChangesAsync();
        }

        public async Task<ReportType> GetByIdAsync(string id)
        {
            return await hFDBPostserviceContext.ReportTypes.FirstAsync(x => x.ReportTypeId == id);
        }

        public async Task<ReportType> GetByPropertyAsync(Expression<Func<ReportType, bool>> predicate)
        {
            var reportType = await hFDBPostserviceContext.ReportTypes.AsNoTracking().FirstOrDefaultAsync(predicate);
            return reportType ?? new ReportType() { 
                ReportTypeId = Ulid.Empty.ToString() 
            };
        }

        public async Task<IEnumerable<ReportType>> GetsAsync()
        {
            return await hFDBPostserviceContext.ReportTypes.ToListAsync();
        }

        public Task<IEnumerable<ReportType>?> GetsByPropertyAsync(Expression<Func<ReportType, bool>> predicate , int size = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, ReportType entity)
        {
            var existingReportType = await hFDBPostserviceContext.ReportTypes.FirstOrDefaultAsync(x => x.ReportTypeId == id);
            if (existingReportType == null) return;
            hFDBPostserviceContext.Entry(existingReportType).CurrentValues.SetValues(entity);
            hFDBPostserviceContext.Entry(existingReportType).State = EntityState.Modified;
            await hFDBPostserviceContext.SaveChangesAsync();
        }
    }
}
