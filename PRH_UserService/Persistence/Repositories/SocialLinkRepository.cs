using System;
using System.Linq.Expressions;
using Application.Interfaces.GenericRepository;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SocialLinkRepository(UserServiceDbContext context) : ISocialLinkRepository
{
    public async Task Create(SocialLink entity)
    {
        await context.SocialLinks.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var link = await context.SocialLinks.FindAsync(id) ?? throw new ArgumentException($"SocialLink with ID {id} not found.");
        context.SocialLinks.Remove(link);
        await context.SaveChangesAsync();
    }

    public async Task<SocialLink?> GetByIdAsync(string id)
    {
        return await context.SocialLinks.FindAsync(id);
    }

    public async Task<SocialLink?> GetByPropertyAsync(Expression<Func<SocialLink, bool>> predicate)
    {
        return await context.SocialLinks.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public Task<IEnumerable<SocialLink>?> GetsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IList<SocialLink>?> GetsByPropertyAsync(Expression<Func<SocialLink, bool>> predicate)
    {
        return await context.SocialLinks.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task UpdateAsync(string id, SocialLink entity)
    {
        var existingSocicalLink = await context.SocialLinks.FindAsync(id) ??
                        throw new ArgumentException($" SocialLink with ID {id} not found.");
        context.Entry(existingSocicalLink).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    Task<IEnumerable<SocialLink>?> IReadRepository<SocialLink>.GetsByPropertyAsync(Expression<Func<SocialLink, bool>> predicate)
    {
        throw new NotImplementedException();
    }
}
