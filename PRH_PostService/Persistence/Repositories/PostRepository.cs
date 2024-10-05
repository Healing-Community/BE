using Application.Interfaces.Repository;
using Domain.Entities;
using System.Linq.Expressions;


namespace Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        public Task Create(Post entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetByPropertyAsync(Expression<Func<Post, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, Post entity)
        {
            throw new NotImplementedException();
        }
    }
}
