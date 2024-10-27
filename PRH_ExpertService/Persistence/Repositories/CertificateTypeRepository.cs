using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class CertificateTypeRepository(ExpertDbContext expertDbContext) : ICertificateTypeRepository
    {
        public async Task Create(CertificateType entity)
        {
            await expertDbContext.CertificateTypes.AddAsync(entity);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var certificateType = await expertDbContext.CertificateTypes.FindAsync(id);
            if (certificateType != null)
            {
                expertDbContext.CertificateTypes.Remove(certificateType);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<CertificateType?> GetByIdAsync(string id)
        {
            return await expertDbContext.CertificateTypes.FindAsync(id);
        }

        public async Task<CertificateType?> GetByPropertyAsync(Expression<Func<CertificateType, bool>> predicate)
        {
            return await expertDbContext.CertificateTypes
                                         .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<CertificateType>> GetsAsync()
        {
            return await expertDbContext.CertificateTypes.ToListAsync();
        }

        public async Task Update(string id, CertificateType entity)
        {
            var existingCertificateType = await expertDbContext.CertificateTypes.FindAsync(id);
            if (existingCertificateType != null)
            {
                expertDbContext.Entry(existingCertificateType).CurrentValues.SetValues(entity);
                await expertDbContext.SaveChangesAsync();
            }
        }
    }
}
