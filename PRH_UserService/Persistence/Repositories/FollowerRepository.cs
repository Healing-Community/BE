using System;
using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class FollowerRepository(UserServiceDbContext context) : IFollowerRepository
{
    public async Task<bool> CheckFollow(string userId, string followerId)
    {
        return await context.Followers.AsNoTracking().AnyAsync(x => x.UserId == userId && x.FollowerId == followerId);
    }

    public async Task Create(Follower entity)
    {
        await context.Followers.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var record = await context.Followers.FindAsync(id) ?? throw new ArgumentException($"Follower with ID {id} not found.");
        context.Followers.Remove(record);
        await context.SaveChangesAsync();
    }

    public async Task<Follower?> GetByIdAsync(string id)
    {
        return await context.Followers.FindAsync(id);
    }

    public async Task<Follower?> GetByPropertyAsync(Expression<Func<Follower, bool>> predicate)
    {
        return await context.Followers.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Follower>?> GetsAsync()
    {
        return await context.Followers.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Follower>?> GetsByPropertyAsync(Expression<Func<Follower, bool>> predicate)
    {
        return await context.Followers.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task UpdateAsync(string id, Follower entity)
    {
        var existingRecord = await context.Followers.FindAsync(id) ?? throw new ArgumentException($"Follower with ID {id} not found.");
        context.Entry(existingRecord).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Follower>?> GetFollowers(string userId)
    {
        return await context.Followers.AsNoTracking().Where(x => x.FollowerId == userId).ToListAsync();
    }
}
