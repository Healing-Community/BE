using System;
using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserReferenceRepository(HFDBPostserviceContext context) : IUserReferenceRepository
{
    public async Task Create(UserPreference entity)
    {
        await context.UserPreferences.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<UserPreference> GetByIdAsync(string id)
    {
        return await context.UserPreferences.FirstAsync(x => x.Id == id);   
    }

    public async Task<UserPreference?> GetByPropertyAsync(Expression<Func<UserPreference, bool>> predicate)
    {
        return await context.UserPreferences.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public Task<IEnumerable<UserPreference>> GetsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserPreference>?> GetsByPropertyAsync(Expression<Func<UserPreference, bool>> predicate , int size = int.MaxValue)
    {
        throw new NotImplementedException();
    }
}
