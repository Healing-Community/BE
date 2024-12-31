using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Repository
{
    public interface IGroupCreationRequestRepository : IReadRepository<GroupCreationRequest>, ICreateRepository<GroupCreationRequest>, IUpdateRepository<GroupCreationRequest>, IDeleteRepository
    {
        Task<IEnumerable<GroupCreationRequest>> GetByPropertyListAsync(Expression<Func<GroupCreationRequest, bool>> predicate);
    }
}
