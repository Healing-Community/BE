using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICertificateRepository : IReadRepository<Certificate>, ICreateRepository<Certificate>, IUpdateRepository<Certificate>, IDeleteRepository
    {
        Task<IEnumerable<Certificate>> GetCertificatesByExpertIdAsync(string expertId);
    }
}
