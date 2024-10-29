using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GroupRepository(HFDBGroupServiceContext hFDBGroupServiceContext) : IGroupRepository
    {
        public async Task Create(Group entity)
        {
            await hFDBGroupServiceContext.Groups.AddAsync(entity);
            await hFDBGroupServiceContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Group> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Group> GetByPropertyAsync(Expression<Func<Group, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Group>> GetsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, Group entity)
        {
            throw new NotImplementedException();
        }
    }
}
