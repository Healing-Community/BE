using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IReactionRepository : IReadRepository<Reaction>, ICreateRepository<Reaction>, IUpdateRepository<Reaction>, IDeleteRepository
    {
    }
}
