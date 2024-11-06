using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPaymentRepository : IReadRepository<Payment>, ICreateRepository<Payment>, IUpdateRepository<Payment>, IDeleteRepository
    {
        Task<Payment?> GetByOrderCodeAsync(string orderCode);
    }
}
