using System;
using System.Linq.Expressions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PlatformFeeRepository(PaymentDbContext context) : IPlatformFeeRepository
{
    public Task Create(PlatformFee entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<PlatformFee?> GetByIdAsync(string id)
    {
        return await context.PlatformFees.FindAsync(id);
    }

    public async Task<PlatformFee?> GetByPropertyAsync(Expression<Func<PlatformFee, bool>> predicate)
    {
        return await context.PlatformFees.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<PlatformFee>> GetsAsync()
    {
        return await context.PlatformFees.ToListAsync();
    }

    public async Task Update(string id, PlatformFee entity)
    {
        var existingPlatformFee = await context.PlatformFees.FindAsync(id) ?? throw new ArgumentException("Platform fee not found");
        context.Entry(existingPlatformFee).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }
}
