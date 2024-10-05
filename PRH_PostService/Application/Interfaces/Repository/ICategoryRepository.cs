using Application.Interfaces.GenericRepository;
using Domain.Entities;
namespace Application.Interfaces.Repository
{
    public interface ICategoryRepository : ICreateRepository<Category>, IReadRepository<Category>, IUpdateRepository<Category>, IDeleteRepository
    {
    }
}
