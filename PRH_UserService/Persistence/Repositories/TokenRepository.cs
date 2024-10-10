using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class TokenRepository(HFDbContext hFDbContext) : ITokenRepository
    {
        public async Task Create(Token entity)
        {
            await hFDbContext.Tokens.AddAsync(entity);
            await hFDbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Token> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Token> GetByPropertyAsync(Expression<Func<Token, bool>> predicate)
        {
            return await hFDbContext.Tokens.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Token();
        }

        public Task<IEnumerable<Token>> GetsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Update(Guid id, Token entity)
        {
            var existingToken = await hFDbContext.Tokens.FindAsync(id) ?? throw new ArgumentException($"Token with ID {id} not found.");
            hFDbContext.Entry(existingToken).CurrentValues.SetValues(entity);
            await hFDbContext.SaveChangesAsync();
        }
    }
}
