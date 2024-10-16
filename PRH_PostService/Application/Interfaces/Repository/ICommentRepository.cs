using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICommentRepository : IReadRepository<Comment>, ICreateRepository<Comment>, IUpdateRepository<Comment>, IDeleteRepository
    {
    }
}
