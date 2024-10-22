using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class CertificateRepository(ExpertDbContext expertDbContext) : ICertificateRepository
    {
        public async Task Create(Certificate entity)
        {
            await expertDbContext.Certificates.AddAsync(entity);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var certificates = await expertDbContext.Certificates.FirstOrDefaultAsync(x => x.CertificateId == id);
            if (certificates == null) return;
            expertDbContext.Certificates.Remove(certificates);
            await expertDbContext.SaveChangesAsync();
        }

        public async Task<Certificate> GetByIdAsync(string id)
        {
            return await expertDbContext.Certificates.FirstAsync(x => x.CertificateId == id);
        }

        public async Task<Certificate> GetByPropertyAsync(Expression<Func<Certificate, bool>> predicate)
        {
            return await expertDbContext.Certificates.AsNoTracking().FirstOrDefaultAsync(predicate)
                   ?? throw new InvalidOperationException("Certificate not found");
        }

        public async Task<IEnumerable<Certificate>> GetCertificatesByExpertIdAsync(string expertId)
        {
            return await expertDbContext.Certificates
                                         .Where(c => c.ExpertProfileId == expertId)
                                         .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetsAsync()
        {
            return await expertDbContext.Certificates.ToListAsync();
        }

        public async Task Update(string id, Certificate entity)
        {
            var existingCertificates = await expertDbContext.Certificates.FirstOrDefaultAsync(x => x.CertificateId == id);
            if (existingCertificates == null) return;
            expertDbContext.Entry(existingCertificates).CurrentValues.SetValues(entity);
            expertDbContext.Entry(existingCertificates).State = EntityState.Modified;
            await expertDbContext.SaveChangesAsync();
        }
    }
}
