using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class RoleRepository(UserServiceDbContext hFDbContext) : IRoleRepository
{
    public async Task Create(Role entity)
    {
        await hFDbContext.Roles.AddAsync(entity);
        await hFDbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Role?> GetByIdAsync(string id)
    {
        return await hFDbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == int.Parse(id));
    }

    public async Task<Role?> GetByPropertyAsync(Expression<Func<Role, bool>> predicate)
    {
        return await hFDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Role>?> GetsAsync()
    {
        return await hFDbContext.Roles.ToListAsync();
    }

    public async Task<IEnumerable<Role>?> GetsByPropertyAsync(Expression<Func<Role, bool>> predicate)
    {
        return await hFDbContext.Roles.AsNoTracking().Where(predicate).ToListAsync();
    }

    public Task UpdateAsync(string id, Role entity)
    {
        throw new NotImplementedException();
    }
}