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

    public Task<Role> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Role> GetByPropertyAsync(Expression<Func<Role, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleNameById(int roleId)
    {
        var role = await hFDbContext.Roles.FirstAsync(r => r.RoleId == roleId);
        return role.RoleName;
    }

    public async Task<IEnumerable<Role>> GetsAsync()
    {
        return await hFDbContext.Roles.ToListAsync();
    }

    public Task Update(string id, Role entity)
    {
        throw new NotImplementedException();
    }
}