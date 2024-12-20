using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IShareRepository : IReadRepository<Share>, ICreateRepository<Share>, IUpdateRepository<Share>, IDeleteRepository
{

}
