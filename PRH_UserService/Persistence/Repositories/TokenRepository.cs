using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class TokenRepository(UserServiceDbContext hFDbContext) : ITokenRepository
{
    public async Task Create(Token entity)
    {
        await hFDbContext.Tokens.AddAsync(entity);
        await hFDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var token = await hFDbContext.Tokens.FindAsync(id) ??
                    throw new ArgumentException($"Token with ID {id} not found.");
        hFDbContext.Tokens.Remove(token);
        await hFDbContext.SaveChangesAsync();
    }

    public Task<Token?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Token?> GetByPropertyAsync(Expression<Func<Token, bool>> predicate)
    {
        return await hFDbContext.Tokens.AsNoTracking().FirstOrDefaultAsync(predicate);
    }


    public Task<IEnumerable<Token>?> GetsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(string id, Token entity)
    {
        var existingToken = await hFDbContext.Tokens.FindAsync(id) ??
                            throw new ArgumentException($"Token with ID {id} not found.");
        hFDbContext.Entry(existingToken).CurrentValues.SetValues(entity);
        await hFDbContext.SaveChangesAsync();
    }
}