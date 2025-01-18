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
            var certificate = await expertDbContext.Certificates.FindAsync(id);
            if (certificate != null)
            {
                expertDbContext.Certificates.Remove(certificate);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<Certificate?> GetByIdAsync(string id)
        {
            return await expertDbContext.Certificates.FindAsync(id);
        }

        public async Task<Certificate?> GetByPropertyAsync(Expression<Func<Certificate, bool>> predicate)
        {
            return await expertDbContext.Certificates
                                         .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Certificate>> GetsAsync()
        {
            return await expertDbContext.Certificates.ToListAsync();
        }

        public async Task Update(string id, Certificate entity)
        {
            var existingCertificate = await expertDbContext.Certificates.FindAsync(id);
            if (existingCertificate != null)
            {
                expertDbContext.Entry(existingCertificate).CurrentValues.SetValues(entity);
                await expertDbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Certificate>> GetCertificatesByExpertIdAsync(string expertProfileId)
        {
            return await expertDbContext.Certificates
                .Where(c => c.ExpertProfileId == expertProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetByExpertProfileIdAsync(string expertProfileId)
        {
            return await expertDbContext.Certificates
                .Where(c => c.ExpertProfileId == expertProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetApprovedCertificatesByExpertIdAsync(string expertProfileId)
        {
            return await expertDbContext.Certificates
                .Where(c => c.ExpertProfileId == expertProfileId && c.Status == 1) // Status 1 means approved
                .ToListAsync();
        }
    }
}
