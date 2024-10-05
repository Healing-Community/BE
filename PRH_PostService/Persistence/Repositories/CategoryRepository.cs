using Application.Interfaces.Repository;
using Domain.Entities;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task Create(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByPropertyAsync(Expression<Func<Category, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
