using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IReportRepository : IReadRepository<Report>, ICreateRepository<Report>, IUpdateRepository<Report>, IDeleteRepository
    {
    }
}
