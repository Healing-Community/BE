using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Persistence.Repositories;

public class UserRepository(UserServiceDbContext hFDbContext) : IUserRepository
{
    public async Task Create(User entity)
    {
        await hFDbContext.Users.AddAsync(entity);
        await hFDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await hFDbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null) return;
        hFDbContext.Users.Remove(user);
        await hFDbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await hFDbContext.Users.FirstAsync(x => x.UserId == id);
    }

    public async Task<User?> GetByPropertyAsync(Expression<Func<User, bool>> predicate)
    {
        return await hFDbContext.Users.AsNoTracking().FirstOrDefaultAsync(predicate) ??
               new User { UserId = Ulid.Empty.ToString() };
    }

    public async Task UpdateAsync(string id, User entity)
    {
        var existingUser = await hFDbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);
        if (existingUser == null) return;
        hFDbContext.Entry(existingUser).CurrentValues.SetValues(entity);
        hFDbContext.Entry(existingUser).State = EntityState.Modified;
        await hFDbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await hFDbContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await hFDbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>?> GetsAsync()
    {
        return await hFDbContext.Users.ToListAsync();
    }

    public async Task<bool> IsUserExistAsync(string userId)
    {
        return await hFDbContext.Users.AnyAsync(u => u.UserId == userId);
    }

    public async Task<IEnumerable<User>?> GetsByPropertyAsync(Expression<Func<User, bool>> predicate)
    {
        return await hFDbContext.Users.Where(predicate).ToListAsync();
    }
}