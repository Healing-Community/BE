using System.Linq.Expressions;
using Application.Interfaces.GenericRepository;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class GenericRepository<T>(UserServiceDbContext context) : IGenericRepository<T> where T : class
{
    public async Task Create(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var record = await context.Set<T>().FindAsync(id) ?? throw new ArgumentException($"{typeof(T).Name} with ID {id} not found.");
        context.Set<T>().Remove(record);
        await context.SaveChangesAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>?> GetsAsync()
    {
        return await context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>?> GetsByPropertyAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var existingRecord = await context.Set<T>().FindAsync(id) ?? throw new ArgumentException($"{typeof(T).Name} with ID {id} not found.");
        context.Entry(existingRecord).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }
}
