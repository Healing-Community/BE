using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IPostRepository : IReadRepository<Post>, ICreateRepository<Post>, IUpdateRepository<Post>, IDeleteRepository
    {
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string postId);
    }
}
