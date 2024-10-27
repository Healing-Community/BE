using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IExpertProfileRepository : IReadRepository<ExpertProfile>, ICreateRepository<ExpertProfile>, IUpdateRepository<ExpertProfile>, IDeleteRepository
    {
    }
}
