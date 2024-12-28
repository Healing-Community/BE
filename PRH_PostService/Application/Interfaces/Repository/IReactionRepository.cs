using Application.Interfaces.GenericRepository;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Repository
{
    public interface IReactionRepository :IReadRepository<Reaction>, ICreateRepository<Reaction>, IUpdateRepository<Reaction>, IDeleteRepository
    {
        Task<IEnumerable<Reaction>> GetsMostReactedPost(int top);
    }
}
