using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICertificateTypeRepository : IReadRepository<CertificateType>, ICreateRepository<CertificateType>, IUpdateRepository<CertificateType>, IDeleteRepository
    {
    }
}
