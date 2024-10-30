using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IUserGroupRepository : IReadRepository<UserGroup>, ICreateRepository<UserGroup>, IUpdateRepository<UserGroup>, IDeleteRepository
    {
    }
}
