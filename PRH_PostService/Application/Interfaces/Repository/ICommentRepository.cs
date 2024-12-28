using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICommentRepository : IReadRepository<Comment>, ICreateRepository<Comment>, IUpdateRepository<Comment>, IDeleteRepository
    {
        Task<bool> ExistsAsync(string id);
        IQueryable<Comment> GetQueryable();
        Task<int> CountCommentsByPostIdAsync(string postId);
        Task<IEnumerable<Comment>> GetAllCommentsByPostIdAsync(string postId);
        Task<IEnumerable<Comment>> GetAllCommentsByCommentIdAsync(string commentId);
        Task<IEnumerable<Comment>> GetAllCommentsByShareIdAsync(string shareId);
    }
}
