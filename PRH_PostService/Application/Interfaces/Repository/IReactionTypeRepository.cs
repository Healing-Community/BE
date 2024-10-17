using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IReactionTypeRepository : IReadRepository<ReactionType>, ICreateRepository<ReactionType>, IUpdateRepository<ReactionType>, IDeleteRepository
    {
    }
}
