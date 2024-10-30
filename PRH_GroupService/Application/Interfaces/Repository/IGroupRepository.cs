using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IGroupRepository : IReadRepository<Group>, ICreateRepository<Group>, IUpdateRepository<Group>, IDeleteRepository
    {
    }
}
