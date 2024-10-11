using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class RoleRepository(HFDbContext hFDbContext) : IRoleRepository
    {
        public async Task Create(Role entity)
        {
            await hFDbContext.Roles.AddAsync(entity);
            await hFDbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetByIdAsync(Guid id)
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

        public Task Update(Guid id, Role entity)
        {
            throw new NotImplementedException();
        }
    }
}
