using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ShareRepository(HFDBPostserviceContext context) : IShareRepository
{
    public async Task Create(Share entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var share = await context.Shares.FindAsync(id);
        if (share == null) return;
        context.Shares.Remove(share);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string shareId)
    {
        return await context.Shares.AnyAsync(s => s.ShareId == shareId);
    }

    public async Task<Share> GetByIdAsync(string id)
    {
        return await context.Shares.FirstAsync(x => x.ShareId == id);
    }

    public async Task<Share?> GetByPropertyAsync(Expression<Func<Share, bool>> predicate)
    {
        return await context.Shares.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Share>> GetsAsync()
    {
        return await context.Shares.ToListAsync();
    }

    public async Task<IEnumerable<Share>?> GetsByPropertyAsync(Expression<Func<Share, bool>> predicate , int size = int.MaxValue)
    {
        return await context.Shares.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task Update(string id, Share entity)
    {
        var existingShare = await context.Shares.FindAsync(id);
        if (existingShare == null) return;
        context.Entry(existingShare).CurrentValues.SetValues(entity);
        context.Entry(existingShare).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
