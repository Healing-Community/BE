using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IFollowerRepository : IReadRepository<Follower>, ICreateRepository<Follower>, IDeleteRepository, IUpdateRepository<Follower>
{
    Task<bool> CheckFollow(string userId, string followerId);
    Task<IEnumerable<Follower>?> GetFollowers(string userId);
}
