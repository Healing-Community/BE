using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IUserRepository : IReadRepository<User>, ICreateRepository<User>, IUpdateRepository<User>,
    IDeleteRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUserNameAsync(string userName);
    Task<bool> IsUserExistAsync(string userId);
    Task<int> CountAsync();
    Task<int> CountNewUsersThisMonthAsync();
    Task<Dictionary<string, int>> CountUsersByRoleAsync();
}